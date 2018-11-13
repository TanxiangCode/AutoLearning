using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace AutoLearning
{
    /// <summary>
    /// 文 件 名：MainForm
    /// 创建人员：谭翔
    /// 创建时间：2018/10/23 14:05:22
    /// 说明描述：
    /// 修改记录：
    /// <para>R1、……</para>
    /// <para>R2、……</para>
    /// </summary>
    public partial class MainForm : Form
    {
        private const string RegistryInfoPath = @"Software\uestcedu";
        private UserInfo _userInfo = new UserInfo();
        private LoginInfo _loginInfo = null;
        private List<LearnData> _learnDataList = null;
        private bool _isInit = false;
        private const double Ver = 1.5;

        public MainForm()
        {
            InitializeComponent();
            this.Text += Ver;
            this.Icon = Icon.ExtractAssociatedIcon(Application.ExecutablePath);
            _loginInfo = new LoginInfo();
            ServicePointManager.DefaultConnectionLimit = 500;
        }

        #region 窗体事件

        private void MainForm_Shown(object sender, EventArgs e)
        {
            try
            {
                RegistryKey key = Registry.CurrentUser.OpenSubKey(RegistryInfoPath);
                if (key != null)
                {
                    char[] separator = new char[] { ',' };
                    string[] strArray = key.GetValue("AccountInfo", "").ToString().Split(separator);
                    if (strArray.Length == 2)
                    {
                        this.txtUserName.Text = strArray[0];
                        this.txtPassWord.Text = strArray[1];
                        this.ckRemember.Checked = true;
                    }
                }
            }
            catch
            {
                // ignored
            }

            Task.Run(() => { CheckUpdate(); });
            this.WriteLog("软件启动成功！");
        }

        private void Login_btn_Click(object sender, EventArgs e)
        {
            this.txtUserName.Enabled = false;
            this.txtPassWord.Enabled = false;
            this.ckRemember.Enabled = false;
            this.btnLogin.Enabled = false;
            _loginInfo.LoginName = txtUserName.Text;
            _loginInfo.Password = txtPassWord.Text;
            _loginInfo.Remember = ckRemember.Checked;

            Task.Run(() =>
            {
                _isInit = true;
                if (Login() && GetUserInfo() && GetRollInfo())
                {
                    RememberPwd();
                    LoadLearnList();
                    LoginLearnCenter();
                    GetUserScore();
                    GetLessonList();
                    this.WriteLog("程序初始化完成...");
                }
                else
                {
                    ShowToForm(() =>
                    {
                        this.txtUserName.Enabled = true;
                        this.txtPassWord.Enabled = true;
                        this.ckRemember.Enabled = true;
                        this.btnLogin.Enabled = true;
                    });
                }
                _isInit = false;
            });

            Task.Run(() =>
            {
                Stopwatch cw = new Stopwatch();
                cw.Start();
                while (true)
                {
                    if (_isInit)
                    {
                        Thread.Sleep(5000);
                        continue;
                    }

                    if (cw.Elapsed.Seconds < 10)
                    {
                        Thread.Sleep(2000);
                        continue;
                    }

                    try
                    {
                        GetUserScore(false);
                    }
                    finally
                    {
                        cw.Restart();
                    }
                }
            });
        }

        private void dataGridView1_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.ColumnIndex == 7)
            {
                if (_isInit)
                {
                    MessageBox.Show("程序初始化中，请稍后...");
                    return;
                }

                var cells = dataGridView.Rows[e.RowIndex].Cells;
                if (cells["dgvlcOperation"].Value.ToString() == "正在学习中" || string.IsNullOrWhiteSpace(cells["dgvlcOperation"].Value.ToString())) return;

                var courseId = cells["dgvtbcCourseId"].Value.ToString();
                if (!string.IsNullOrWhiteSpace(courseId))
                {
                    cells["dgvlcOperation"].Value = "正在学习中";
                    var learnTask = Task.Run(() => { BeginLearn(courseId); });
                    learnTask.ContinueWith(t =>
                    {
                        ShowToForm(() =>
                        {
                            cells["dgvlcOperation"].Value = "开始学习";
                        });
                    });
                }
            }
            else if (e.ColumnIndex == 8)
            {
                var downloadUrl = dataGridView.Rows[e.RowIndex].Cells["dgvtbcAddress"].Value.ToString();
                if (!string.IsNullOrWhiteSpace(downloadUrl))
                {
                    saveFileDialog1.FileName = Path.GetFileName(downloadUrl);
                    if (saveFileDialog1.ShowDialog() == DialogResult.OK)
                    {
                        this.WriteLog("正在下载资料中...");
                        WebClient webClient = new WebClient();
                        webClient.DownloadFileAsync(new Uri(downloadUrl, UriKind.Absolute), saveFileDialog1.FileName);
                        webClient.DownloadFileCompleted += (ss, ee) =>
                        {
                            this.WriteLog($"下载资料完成!!! 位置：{saveFileDialog1.FileName}");
                        };
                    }
                }
            }
        }

        private void lvLog_DrawItem(object sender, DrawItemEventArgs e)
        {
            e.Graphics.FillRectangle(new SolidBrush(e.BackColor), e.Bounds);
            if (e.Index >= 0)
            {
                StringFormat stringFormat = new StringFormat
                {
                    LineAlignment = StringAlignment.Center
                };
                var str = ((ListBox)sender).Items[e.Index].ToString();
                var strList = str.Split(new[] { "|" }, StringSplitOptions.None);
                Color color = strList[0] == "0" ? Color.BlueViolet : Color.FromName(strList[0]);
                e.Graphics.DrawString(strList[1], e.Font, new SolidBrush(color), e.Bounds, stringFormat);
            }
            e.DrawFocusRectangle();
        }

        private void lvLog_MeasureItem(object sender, MeasureItemEventArgs e)
        {
            e.ItemHeight = e.ItemHeight + 8;
        }

        #endregion 窗体事件

        private void RememberPwd()
        {
            try
            {
                Registry.CurrentUser.DeleteSubKey(RegistryInfoPath);
            }
            catch
            {
                // ignored
            }

            if (_loginInfo.Remember)
            {
                Registry.CurrentUser.CreateSubKey(RegistryInfoPath)?.SetValue("AccountInfo", $"{_loginInfo.LoginName},{_loginInfo.Password}");
            }
        }

        private void CheckUpdate()
        {
            this.WriteLog("检查程序更新中...");

            string fileName = null;
            using (WebClient verClient = new WebClient())
            {
                byte[] bytes = verClient.DownloadData("http://www.flebt.cn/AutoLearning/version.txt");
                var verFile = UTF8Encoding.UTF8.GetString(bytes);
                var verStrings = verFile.Split(new[] { "\r\n" }, StringSplitOptions.None);
                if (verStrings.Length > 1)
                {
                    if (Convert.ToDouble(verStrings[0]) > Ver)
                    {
                        fileName = verStrings[1];
                    }
                }
            }

            if (string.IsNullOrWhiteSpace(fileName))
            {
                this.WriteLog("已是最新版本...");
                return;
            }

            if (MessageBox.Show("当前程序有更新？需要更新么？", "提示", MessageBoxButtons.YesNo) == DialogResult.Yes)
            {
                using (WebClient client = new WebClient())
                {
                    client.DownloadFileCompleted += delegate (object sender, AsyncCompletedEventArgs e)
                    {
                        if (e.Error == null)
                        {
                            var exeName = Path.GetFileName(Application.ExecutablePath);
                            StringBuilder bat = new StringBuilder();
                            bat.AppendLine("@echo off ");
                            bat.AppendLine($"taskkill /f /im {exeName}");
                            bat.AppendLine("CHOICE /T 2 /C ync /CS /D y /n ");
                            bat.AppendLine($"Del /F /Q {exeName}");
                            bat.AppendLine($"rename {exeName}.new {exeName}");
                            bat.AppendLine($"start {exeName}");
                            bat.AppendLine("del %0");
                            File.WriteAllText("update.bat", bat.ToString());
                            Process.Start("update.bat");
                        }
                    };
                    var newFileName = fileName;
                    if (Path.GetFileName(Application.ExecutablePath) == fileName)
                    {
                        newFileName = $"{fileName}.new";
                    }

                    var url = $"http://www.flebt.cn/AutoLearning/{fileName}";
                    client.DownloadFileAsync(new Uri(url, UriKind.Absolute), Path.Combine(Application.StartupPath, newFileName));
                }
            }
        }

        private bool Login()
        {
            WriteLog("登录管理系统中...");

            //验证码为欺骗性质，可直接跳过
            //var verifyUrl = $"http://www.uestcedu.com/ifree/VerifyUtil.do?{Util.GetRandomDouble()}";
            //var tuple = Util.HttpRtImg(verifyUrl, "", null);
            //string verifyCode = "";

            //using (var frm = new FrmVerify())
            //{
            //    frm.SetImage(tuple.Item1);
            //    if (frm.ShowDialog() == DialogResult.OK)
            //    {
            //        verifyCode = frm.GetInputVerify();
            //    }
            //}

            //if (string.IsNullOrWhiteSpace(verifyCode))
            //{
            //    WriteLog("请输入验证码！！！", Color.Red);
            //    return false;
            //}

            //WriteLog("效验数据中...");
            //var opLoginUrl = $"http://www.uestcedu.com/ifree/servlet/com.lemon.web.ActionServlet?handler=com.ifree.system.user.UserLoginAction&op=login&_no_html=1&{Util.GetRandomDouble()}";
            //var opLoginBody = $"txtLoginName={_loginInfo.LoginName}&txtPassword={_loginInfo.Password}&txtVerifyCode={verifyCode}&chkSaveInfo=0&selSaveTime=undefined&ran={Util.GetRandomDouble()}";
            //var vHtml = Util.HttpRtHtml(opLoginUrl, opLoginBody, tuple.Item2);
            //if (vHtml.Contains("message=验证码无效"))
            //{
            //    WriteLog("验证码无效！！！", Color.Red);
            //    return false;
            //}
            WriteLog("验证码效验中...");

            var loginUrl = "http://student.uestcedu.com/login.jsp";
            var body = $"txtLoginName={_loginInfo.LoginName}&txtPassword={_loginInfo.Password}&ran={Util.GetRandomDouble()}&ChkVerifyCode=no&{Util.GetRandomInt()}";
            _userInfo.LoginCookie = Util.HttpRtCookie(loginUrl, body, "GET");

            if (_userInfo.LoginCookie == null || _userInfo.LoginCookie.Count == 0)
            {
                WriteLog("登录失败!!!", Color.Red);
                return false;
            }
            return true;
        }

        private bool GetUserInfo()
        {
            this.WriteLog("获取用户信息...");
            var userInfoUrl = $"http://student.uestcedu.com/console/user_info.jsp?{Util.GetRandomDouble()}";
            var userInfoJson = Util.HttpRtHtml(userInfoUrl, "", _userInfo.LoginCookie, "GET");
            var userInfo = Util.JsonToObject<UserInfo>(userInfoJson);
            _userInfo.user_name = userInfo.user_name;
            _userInfo.login_name = userInfo.login_name;
            _userInfo.user_id = userInfo.user_id;

            if (string.IsNullOrWhiteSpace(_userInfo.user_id))
            {
                WriteLog("获取用户信息失败!!!", Color.Red);
                return false;
            }
            return true;
        }

        private bool GetRollInfo()
        {
            this.WriteLog("获取学籍信息...");
            var rollInfoUrl = "http://student.uestcedu.com/console/apply/student/roll_student_list.jsp";
            var rollInfoHtml = Util.HttpRtHtml(rollInfoUrl, "", _userInfo.LoginCookie, "GET");
            var rollInfoDoc = ConvertToDocument(rollInfoHtml);
            var tblDataList = rollInfoDoc.GetElementbyId("tblDataList");
            if (tblDataList == null)
            {
                this.WriteLog("获取学籍信息失败!!!", Color.Red);
                return false;
            }
            var tblRow = tblDataList.ChildNodes.FirstOrDefault(m => !string.IsNullOrWhiteSpace(m.Id) && m.InnerText.Contains("激活"));
            if (tblRow != null)
            {
                var tblCols = tblRow.ChildNodes;
                _userInfo.EnrollCode = tblCols[1].Attributes["EnrollCode"].Value;
                _userInfo.Competent = tblCols[7].InnerText;
            }

            ShowToForm(() =>
            {
                lblLoginInfo.Text = $@"{_userInfo.user_name} -> {_userInfo.Competent}";
            });
            return true;
        }

        private bool LoadLearnList(bool isShowLog = true)
        {
            if (isShowLog)
            {
                this.WriteLog("获取科目列表中...");
            }

            var learnUrl = "http://student.uestcedu.com/console/apply/student/student_learn.jsp";
            var learnHtml = Util.HttpRtHtml(learnUrl, "", _userInfo.LoginCookie, "GET");
            var learnDoc = ConvertToDocument(learnHtml);
            var tblDataList = learnDoc.GetElementbyId("tblDataList");
            if (tblDataList == null)
            {
                this.WriteLog("获取科目列表失败!!!", Color.Red);
                return false;
            }
            var tblRows = tblDataList.ChildNodes.Where(m => !string.IsNullOrWhiteSpace(m.Id));

            _learnDataList = new List<LearnData>();
            foreach (var row in tblRows)
            {
                var tblCols = row.ChildNodes;
                if (tblCols.Count < 9) continue;

                var learnData = new LearnData()
                {
                    No = tblCols[1].InnerText,
                    Course = tblCols[2].InnerText.Replace("\n", ""),
                    CourseId = Util.RegexValue(tblCols[8].InnerHtml, @"(?<=enterLearning\(').*?(?=')"),
                    Address = Util.RegexValue(tblCols[8].InnerHtml, @"(?<=download\(').*?(?=')"),
                    Grade = tblCols[7].InnerText,
                    Semester = tblCols[3].InnerText
                };
                learnData.ExamCode = Util.RegexValue(tblCols[8].InnerHtml, $@"(?<=','{learnData.CourseId}',').*?(?='\))");
                learnData.Operation = tblCols[8].InnerText.Contains("开始学习") ? "开始学习" : "";
                if (!string.IsNullOrWhiteSpace(learnData.Address))
                {
                    learnData.Data = "下载资料";
                }

                _learnDataList.Add(learnData);
            }

            if (_learnDataList.Count == 0)
            {
                this.WriteLog("获取科目列表失败!!!", Color.Red);
                return false;
            }

            ShowToForm(() => { this.dataGridView.DataSource = _learnDataList; });
            return true;
        }

        private bool LoginLearnCenter(bool isShowLog = true)
        {
            if (isShowLog)
            {
                this.WriteLog("登录学习中心...");
            }

            var loginUrl = $"http://learning.uestcedu.com/learning3/login.jsp?op=execscript&urlto=&ip_exp_prefix=%40%40IPEXP%5f" +
                           $"&ip_replace_exp=http%3a%2f%2fwww%2eremotedu%2ecom%2flearning%5fupload%0aSCORM%5fUPLOAD%5fWEB%0ahttp%3a%2f%2flearning" +
                           $"%2euestcedu%2ecom%2flearning3%0aLEARNING%5fWEB%0ahttp%3a%2f%2flearning%2euestcedu%2ecom%2ffiles%2flearning" +
                           $"%0aFILES%5fWEB%5fUPLOAD%5fFOLDER%0ahttp%3a%2f%2flearning%2euestcedu%2ecom%2ffiles%2flearning%0aFILES%5fWEB%5fUOLOAD%5fFOLDER" +
                           $"%0ahttp%3a%2f%2flearning%2euestcedu%2ecom%2ffiles%0aFILES%5fWEB&{Util.GetRandomInt()}";
            var loginBody = $"txtLoginName={_userInfo.EnrollCode}&txtPassword={_loginInfo.Password}&txtCourseId=&txtUserType=student&txtClassId=&txtClassName=&txtSiteId=20";
            _userInfo.LearnEduCookies = Util.HttpRtCookie(loginUrl, loginBody);

            if (_userInfo.LearnEduCookies == null || _userInfo.LearnEduCookies.Count == 0)
            {
                WriteLog("登录学习中心失败!!!", Color.Red);
                return false;
            }

            return true;
        }

        private void GetUserScore(bool isShowLog = true)
        {
            if (isShowLog)
            {
                this.WriteLog("获取学习进度中...");
            }

            if (_learnDataList == null)
            {
                this.WriteLog("无课程信息，正在重新获取科目列表...", Color.Orange);

                if (LoadLearnList(false)) return;
            }

            if (_userInfo.LearnEduCookies == null || _userInfo.LearnEduCookies.Count == 0)
            {
                this.WriteLog("无学习中心登录信息，正在重新登录学习中心...", Color.Orange);

                if (LoginLearnCenter(false)) return;
            }

            var userScoreUrl = "http://learning.uestcedu.com/learning3/course/ajax_user_score.jsp";

            Parallel.For(0, dataGridView.Rows.Count, delegate (int i)
            {
                var learnData = _learnDataList.FirstOrDefault(m => m.CourseId == dataGridView.Rows[i].Cells["dgvtbcCourseId"].Value.ToString());
                if (learnData == null)
                    return;

                var body = $"course_id={learnData.CourseId}&show_icon=1&r={Util.GetRandomDouble()}&{Util.GetRandomInt()}";
                var scoreHtml = Util.HttpRtHtml(userScoreUrl, body, _userInfo.LearnEduCookies, "get");
                if (scoreHtml.Contains("未开始学习！"))
                    return;

                var scoreDoc = ConvertToDocument(scoreHtml);
                var scoreText = scoreDoc.DocumentNode?.InnerText;
                learnData.Progress = Util.RegexValue(scoreText, @"(?<=进度：).*?(?=成绩：)");
                learnData.Score = Util.RegexValue(scoreText, @"(?<=成绩：).*?(?=时间：)");
                learnData.LearnTime = Util.RegexValue(scoreText, @"(?<=时间：).*");
                ShowToForm(() =>
                {
                    this.dataGridView.Rows[i].Cells["dgvtbcProgress"].Value = learnData.Progress;
                    this.dataGridView.Rows[i].Cells["dgvtbcScore"].Value = learnData.Score;
                    this.dataGridView.Rows[i].Cells["dgvtbcLearnTime"].Value = learnData.LearnTime;
                    this.dataGridView.InvalidateRow(i);
                });
            });
        }

        private void GetLessonList(bool isShowLog = true, string courseId = null)
        {
            foreach (var learnData in (string.IsNullOrWhiteSpace(courseId) ? _learnDataList.Where(m => !string.IsNullOrWhiteSpace(m.Operation)) : _learnDataList.Where(m => m.CourseId == courseId && !string.IsNullOrWhiteSpace(m.Operation))))
            {
                if (isShowLog)
                {
                    this.WriteLog($"正在获取【{learnData.Course}】的课程数据...");
                }

                var baseUrl = $"http://learning.uestcedu.com/learning3/course/ajax_learn_content.jsp?course_id={learnData.CourseId}&parent_id={{0}}&content_type=&flag=2&b_edit=0&r={Util.GetRandomDouble()}&{Util.GetRandomInt()}";
                var lessonInfos = typeof(Lesson).GetProperties(BindingFlags.Instance | BindingFlags.Public);
                var lessonList = new List<Lesson>();
                for (var i = 0; i < 15; i++)
                {
                    var learnHtml = Util.HttpRtHtml(string.Format(baseUrl, i), "", _userInfo.LearnEduCookies, "Get");
                    if (string.IsNullOrWhiteSpace(learnHtml) || learnHtml.Contains("无内容。")) continue;

                    var learnDoc = ConvertToDocument(learnHtml.Replace("<td></table>", "<td></tr></table></td></tr>"));
                    var tblDataList = learnDoc.GetElementbyId("tblDataList");
                    if (tblDataList == null) continue;

                    var tblRows = tblDataList.ChildNodes.Where(m => m.Name == "tr");
                    foreach (var row in tblRows)
                    {
                        var baseData = row.ChildNodes.FirstOrDefault(m => m.Name == "td");
                        if (baseData == null) continue;

                        var lesson = new Lesson();
                        foreach (var attribute in baseData.Attributes)
                        {
                            var pi = lessonInfos.FirstOrDefault(m => m.Name == attribute.OriginalName);
                            pi?.SetValue(lesson, attribute.Value);
                        }
                        lesson.Status = Util.RegexValue(row.InnerHtml, @"(?<=class=""scorm ).*?(?="">)");
                        lesson.Name = row.InnerText.Replace("&nbsp;", "").Replace("\n", "");
                        lessonList.Add(lesson);
                    }
                }

                learnData.Lessons = lessonList;
                if (isShowLog)
                {
                    this.WriteLog($"获取【{learnData.Course}】课程共计{lessonList.Count}节!!!", Color.RoyalBlue);
                }
                Thread.Sleep(1000);
            }
        }

        private void BeginLearn(string courseId)
        {
            var learnData = _learnDataList.FirstOrDefault(m => m.CourseId == courseId);
            if (learnData == null)
            {
                this.WriteLog("科目ID有误，无法开始学习!!!", Color.Red);
                return;
            }

            this.WriteLog($"正在进入课程【{learnData.Course}】...");
            var enterInCourseUrl = "http://learning.uestcedu.com/learning3/course/enter_in_course.jsp";
            var enterInCourseBody = $"txtLoginName={_userInfo.EnrollCode}&txtPassword=&txtCourseId={courseId}&txtUserType=student&txtClassId=&txtClassName=&txtSiteId=20";
            Util.HttpNoReturn(enterInCourseUrl, enterInCourseBody, _userInfo.LearnEduCookies);

            this.WriteLog($"开始学习【{learnData.Course}】的课程...");

            if (learnData.Lessons == null)
            {
                this.WriteLog("无课程信息，正在重新获取!!!", Color.Orange);
                GetLessonList();
                if (learnData.Lessons == null) return;
            }

            var user_Id = "";
            foreach (var lesson in learnData.Lessons.Where(m => m.Status == "notattempt" || m.Status == "incomplete"))
            {
                this.WriteLog($"正在学习课程【{learnData.Course}】-【{lesson.Name}】...");
                var loadScoUrl = $"http://learning.uestcedu.com/learning3/scorm/scoplayer/load_sco.jsp?r={Util.GetRandomDouble()}&course_id={lesson.CourseId}&content_id={lesson.ContentId}";
                var loadScoHtml = Util.HttpRtHtml(loadScoUrl, "", _userInfo.LearnEduCookies, "get");
                var userId = Util.RegexValue(loadScoHtml, @"(?<=organHandle.userId="").*?(?="";)");
                var coursewareId = Util.RegexValue(loadScoHtml, @"(?<=organHandle.coursewareId="").*?(?="";)");
                var itemId = Util.RegexValue(loadScoHtml, @"(?<=scoHandle.ItemId="").*?(?="";)");
                //var studentId = Util.RegexValue(loadScoHtml, @"(?<=LMSSetValue(""cmi.core.student_id"","").*?(?="")");
                //var studentName = Util.RegexValue(loadScoHtml, @"(?<=LMSSetValue(""cmi.core.student_name"","").*?(?="");)");

                if (string.IsNullOrWhiteSpace(user_Id))
                    user_Id = userId;
                if (string.IsNullOrWhiteSpace(userId) || string.IsNullOrWhiteSpace(coursewareId) || string.IsNullOrWhiteSpace(itemId))
                {
                    this.WriteLog($"学习课程【{learnData.Course}】-【{lesson.Name}】失败!!!", Color.Red);
                    continue;
                }

                var commitTxt = Util.UrlEncode($"cmi.core.student_id={userId}&cmi.core.student_name={_userInfo.EnrollCode}&cmi.core.lesson_location=&cmi.core.credit=&cmi.core.lesson_status=completed&cmi.core.entry=&" +
                                               $"cmi.core.total_time={new Random().Next(200, 500)}&cmi.core.lesson_mode=&cmi.core.exit=&cmi.core.session_time=10&cmi.core.score.raw=10&cmi.core.score.min=0&cmi.core.score.max=10&" +
                                               $"cmi.comments=&cmi.comments_from_lms=56B4FC7F71AEA501294A4442CD11E13918052709D92BF92CF5476F5920050B43FDA223B4F5C0A1B4AE6F3EA4310D1830500C2DC2DE1E1C3501FAFBCA7A640A92A829EE789B409A87&" +
                                               $"cmi.launch_data=&cmi.student_data.mastery_score=0&cmi.student_data.max_time_allowed=0&cmi.student_data.time_limit_action=&cmi.student_preference.audio=0&" +
                                               $"cmi.student_preference.language=&cmi.student_preference.speed=0&cmi.student_preference.text=0&cmi.suspend_data=&cmi.interactions.0.id=&cmi.interactions.0.time=180&" +
                                               $"cmi.interactions.0.type=content&cmi.interactions.0.weighting=0&cmi.interactions.0.student_response=&cmi.interactions.0.result=&cmi.interactions.0.latency=0");
                var commitUrl = $"http://ispace.remotedu.com/ispace2_sync/scormplayer/commit.htm?{Util.GetRandomDouble()}";
                var scriptHref = $"http://learning.uestcedu.com/learning3/scorm/scoplayer/after_commit.jsp?item_id={itemId}&url={Util.UrlEncode(commitUrl)}";
                var learnUrl = $"http://learning.uestcedu.com/learning3_sync/servlet/com.lemon.web.ActionServlet?handler={Util.UrlEncode("com.lemon.scorm.ScormWebServlet")}&op=commit_data&script={Util.UrlEncode($"window.location.href=\"{scriptHref}\";", null)}&_no_html=0&{Util.GetRandomInt()}";
                var body = $"txtUserId={userId}&txtSCOType=sco&txtCoursewareId={coursewareId}&txtItemId={itemId}&txtCommit={commitTxt}";
                Util.HttpNoReturn(learnUrl, body, _userInfo.LearnEduCookies);
                Thread.Sleep(2000);
            }

            GetLessonList(false, courseId);
            this.WriteLog($"课程【{learnData.Course}】学习完成!!!", Color.RoyalBlue);

            this.WriteLog($"正在生成【{learnData.Course}】的学习分数...");
            var generateRegualrScoreUrl = "http://learning.uestcedu.com/learning3/servlet/com.lemon.web.ActionServlet";
            var generateRegualrScore = $"handler=com%2elemon%2elearning%2ecourse%2eCourseScoreAction&op=generate_regualr_score&course_id={learnData.CourseId}&calc_single=1&user_id={user_Id}&_no_html=0&r={Util.GetRandomDouble()}&{Util.GetRandomInt()}";
            Util.HttpNoReturn(generateRegualrScoreUrl, generateRegualrScore, _userInfo.LearnEduCookies, "get");

            AppraiseCourse(courseId);
            RefreahScore();
        }

        private void AppraiseCourse(string courseId)
        {
            var learnData = _learnDataList.FirstOrDefault(m => m.CourseId == courseId);
            if (learnData == null)
            {
                this.WriteLog("科目ID有误，无法进行评论!!!", Color.Red);
                return;
            }

            this.WriteLog($"开始对课程【{learnData.Course}】进行评论...");

            var appraiseUrl = "http://student.uestcedu.com/console/apply/student/appraise_course.jsp";
            var appraiseBody = $"enroll_code={_userInfo.EnrollCode}&learning_course_id={learnData.CourseId}&exam_code={learnData.ExamCode}&dlg=1&returl=&page={new Random().Next(1, 5000)}&{Util.GetRandomInt()}";
            var appraiseHtml = Util.HttpRtHtml(appraiseUrl, appraiseBody, _userInfo.LoginCookie, "get");
            var appraiseDoc = ConvertToDocument(appraiseHtml);
            var tblDataList = appraiseDoc.GetElementbyId("tblDataList");
            var content = "";
            if (tblDataList == null)
            {
                this.WriteLog("获取评论失败，将评论为固定内容!!!", Color.Orange);
                content = "课件讲解非常好，表达严谨、流畅、准确。";
            }
            else
            {
                var appraises = Regex.Matches(tblDataList.InnerHtml, "(?<=评论：).*?(?=</div>)");
                content = appraises[new Random().Next(0, appraises.Count - 1)].Value;
                var itemLevels = appraiseDoc.DocumentNode.SelectNodes("//input[@name='txtItemLevel']");
                var level = itemLevels.Sum(m => Convert.ToInt32(m.Attributes["value"].Value) * Convert.ToInt32(m.Attributes["item_score"].Value));
                if (level > 0)
                {
                    this.WriteLog($"课程【{learnData.Course}】已评论过，跳过评论!!!", Color.Orange);
                    return;
                }
            }

            var getIpUrl = "https://ip.cn";
            var getIpHtml = Util.HttpRtHtml(getIpUrl, "", null, "get");
            var getIpDoc = ConvertToDocument(getIpHtml);
            var getIpResult = getIpDoc.GetElementbyId("result");
            var thisIP = "";
            if (getIpResult == null)
            {
                thisIP = "0.0.0.0";
            }
            else
            {
                thisIP = getIpResult.SelectSingleNode("//code").InnerText;
            }

            var commitUrl = $"http://student.uestcedu.com/servlet/com.lemon.web.ActionServlet?handler=com.ifree.teachplan.LearningAppraiseCourseAction&op=new_learning_appraise_course&appraise_id=-1&web_form_action=1&script=window.parent.webForm.afterAction('new')%3B&{Util.GetRandomDouble()}";
            var commitBody = $"txtParentId=-1&txtIsNew=1&txtLearningCourseId={learnData.CourseId}&txtExamCode={learnData.ExamCode}&txtLearningCourseName=&txtEnrollCode={_userInfo.EnrollCode}&" +
                             $"txtStudentNo={_userInfo.login_name}&txtStudentName={_userInfo.user_name}&txtItemLevel=5&txtItemLevel=5&txtItemLevel=5&txtItemLevel=5&txtItemLevel=5&" +
                             $"txtItemLevel=5&txtItemLevel=5&txtItemLevel=5&txtItemLevel=5&txtItemLevel=5&txtItemAppraiseInfo=1,5,2,5,3,5,4,5,5,5,6,5,7,5,8,5,9,5,10,5&" +
                             $"txtAppraiseLevel=100&txtContent={content}&txtCreateTime=GETDATE()&txtCreator={_userInfo.login_name}&txtIpAddr={thisIP}&" +
                             $"txtSpecialInfo=txtItemAppraiseInfo&_txtFormObjects=*%26*N%3D%2C90*U%3Armrnr*%26*U%2BP9%27rmrnr*%26*R9%3D%2C0507%5B%2F%29%2C%2B9U%3Armrnr*%26*Y%26%3D1%5B%2F%3A9rmrnr*%26*R9%3D%2C0507%5B%2F%29%2C%2B9P%3D19rmrnr*%26*K*%29%3A90*P%2Frmrnr*%26*K*%29%3A90*P%3D19rmrnr*%26*%5D..%2C%3D5%2B9R9%2892rmrnr*%26*%5B%2F0*90*rmrnr*%26*%5B%2C9%3D*9J519rkrnr*%26*%5B%2C9%3D*%2F%2Crmrnr*%26*U.%5D%3A%3A%2Crmrn&_txtIdFields=%3D..%2C%3D5%2B9%3F5%3A&_txtIndexFields=&_txtIndexFilters=&_txtRemarkFields=&_txtRadioFields=&_txtRadioFilters=&_txtIdValues=-1&_txtIdNumeric=m&_txtTableName=29%3D%2C0507%3F%3D..%2C%3D5%2B9%3F%3B%2F%29%2C%2B9&_txtDebug=0&_txtDataSourceProvider=";

            Util.HttpNoReturn(commitUrl, commitBody, _userInfo.LoginCookie);
            this.WriteLog($"课程【{learnData.Course}】评论完成!!!", Color.RoyalBlue);
        }

        private void RefreahScore()
        {
            this.WriteLog("开始更新成绩...");

            var updateScoreUrl = "http://student.uestcedu.com/servlet/com.lemon.web.ActionServlet";
            var updateScoreBody = $"handler=com%2estudent%2eroll%2escore%2eStudentScoreAction&op=update_student_score&_no_html=1&{Util.GetRandomDouble()}&enroll_code={_userInfo.EnrollCode}&ran={Util.GetRandomDouble()}";
            Util.HttpNoReturn(updateScoreUrl, updateScoreBody, _userInfo.LoginCookie, "get");

            var updateResultUrl = $"http://student.uestcedu.com/console/apply/student/student_learn.jsp?update_time={DateTime.Now.ToString()}&is_sync=1&{Util.GetRandomInt()}";
            var updateResultHtml = Util.HttpRtHtml(updateResultUrl, "", _userInfo.LoginCookie, "get");
            var updateResultDoc = ConvertToDocument(updateResultHtml);
            var tblDataList = updateResultDoc.GetElementbyId("tblDataList");
            if (tblDataList == null)
            {
                this.WriteLog("更新成绩失败!!!", Color.Red);
                return;
            }
            var tblRows = tblDataList.ChildNodes.Where(m => !string.IsNullOrWhiteSpace(m.Id));

            foreach (var row in tblRows)
            {
                var tblCols = row.ChildNodes;
                if (tblCols.Count < 9) continue;

                var courseId = Util.RegexValue(tblCols[8].InnerHtml, @"(?<=enterLearning\(').*?(?=')");
                var grade = tblCols[7].InnerText;
                var learnData = _learnDataList.FirstOrDefault(m => m.CourseId == courseId);
                if (learnData != null)
                {
                    learnData.Grade = grade;
                }
            }

            foreach (DataGridViewRow row in dataGridView.Rows)
            {
                var courseId = row.Cells["dgvtbcCourseId"].Value.ToString();
                var learnData = _learnDataList.FirstOrDefault(m => m.CourseId == courseId);
                if (learnData != null)
                {
                    ShowToForm(() =>
                    {
                        row.Cells["dgvtbcGrade"].Value = learnData.Grade;
                    });
                }
            }
            this.WriteLog("完成更新成绩!!!", Color.RoyalBlue);
        }

        private HtmlAgilityPack.HtmlDocument ConvertToDocument(string html)
        {
            var doc = new HtmlAgilityPack.HtmlDocument();
            doc.LoadHtml(html);
            return doc;
        }

        private void WriteLog(string log, Color color = default(Color))
        {
            ShowToForm(() =>
            {
                this.lvLog.Items.Add($"{color.Name}|{log}");
                this.lvLog.TopIndex = this.lvLog.Items.Count - 1;
            });
        }

        private void ShowToForm(Action action)
        {
            this.Invoke(action);
            Thread.Sleep(1);
            Application.DoEvents();
        }

        private void linkLabel1_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            Process.Start("https://github.com/TanxiangCode/AutoLearning");
        }
    }
}