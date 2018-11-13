using System;
using System.Drawing;
using System.Windows.Forms;

namespace AutoLearning
{
    partial class MainForm
    {
        /// <summary>
        /// 必需的设计器变量。
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// 清理所有正在使用的资源。
        /// </summary>
        /// <param name="disposing">如果应释放托管资源，为 true；否则为 false。</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows 窗体设计器生成的代码

        /// <summary>
        /// 设计器支持所需的方法 - 不要修改
        /// 使用代码编辑器修改此方法的内容。
        /// </summary>
        private void InitializeComponent()
        {
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle3 = new System.Windows.Forms.DataGridViewCellStyle();
            this.btnLogin = new System.Windows.Forms.Button();
            this.lblUserName = new System.Windows.Forms.Label();
            this.lblPwd = new System.Windows.Forms.Label();
            this.txtUserName = new System.Windows.Forms.TextBox();
            this.txtPassWord = new System.Windows.Forms.TextBox();
            this.dataGridView = new System.Windows.Forms.DataGridView();
            this.dgvtbcAddress = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.dgvtbcExamCode = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.dgvtbcCourseId = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.dgvtbcNo = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.dgvtbcSemester = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.dgvtbcCourse = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.dgvtbcGrade = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.dgvtbcProgress = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.dgvtbcScore = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.dgvtbcLearnTime = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.dgvlcOperation = new System.Windows.Forms.DataGridViewLinkColumn();
            this.dgvlcData = new System.Windows.Forms.DataGridViewLinkColumn();
            this.ckRemember = new System.Windows.Forms.CheckBox();
            this.lvLog = new System.Windows.Forms.ListBox();
            this.lblLoginInfo = new System.Windows.Forms.Label();
            this.saveFileDialog1 = new System.Windows.Forms.SaveFileDialog();
            this.label1 = new System.Windows.Forms.Label();
            this.linkLabel1 = new System.Windows.Forms.LinkLabel();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView)).BeginInit();
            this.SuspendLayout();
            // 
            // btnLogin
            // 
            this.btnLogin.Location = new System.Drawing.Point(493, 15);
            this.btnLogin.Name = "btnLogin";
            this.btnLogin.Size = new System.Drawing.Size(97, 24);
            this.btnLogin.TabIndex = 2;
            this.btnLogin.Text = "登录账号";
            this.btnLogin.UseVisualStyleBackColor = true;
            this.btnLogin.Click += new System.EventHandler(this.Login_btn_Click);
            // 
            // lblUserName
            // 
            this.lblUserName.AutoSize = true;
            this.lblUserName.Location = new System.Drawing.Point(22, 21);
            this.lblUserName.Name = "lblUserName";
            this.lblUserName.Size = new System.Drawing.Size(53, 12);
            this.lblUserName.TabIndex = 5;
            this.lblUserName.Text = "用户名：";
            // 
            // lblPwd
            // 
            this.lblPwd.AutoSize = true;
            this.lblPwd.Location = new System.Drawing.Point(242, 21);
            this.lblPwd.Name = "lblPwd";
            this.lblPwd.Size = new System.Drawing.Size(53, 12);
            this.lblPwd.TabIndex = 6;
            this.lblPwd.Text = "密　码：";
            // 
            // txtUserName
            // 
            this.txtUserName.Location = new System.Drawing.Point(81, 17);
            this.txtUserName.Name = "txtUserName";
            this.txtUserName.Size = new System.Drawing.Size(146, 21);
            this.txtUserName.TabIndex = 7;
            // 
            // txtPassWord
            // 
            this.txtPassWord.Location = new System.Drawing.Point(301, 17);
            this.txtPassWord.Name = "txtPassWord";
            this.txtPassWord.PasswordChar = '*';
            this.txtPassWord.Size = new System.Drawing.Size(132, 21);
            this.txtPassWord.TabIndex = 8;
            // 
            // dataGridView
            // 
            this.dataGridView.AllowUserToAddRows = false;
            this.dataGridView.AllowUserToDeleteRows = false;
            this.dataGridView.AllowUserToResizeRows = false;
            this.dataGridView.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.dataGridView.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.Fill;
            this.dataGridView.BackgroundColor = System.Drawing.SystemColors.Control;
            this.dataGridView.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.dataGridView.CellBorderStyle = System.Windows.Forms.DataGridViewCellBorderStyle.Raised;
            dataGridViewCellStyle3.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleCenter;
            dataGridViewCellStyle3.BackColor = System.Drawing.SystemColors.Control;
            dataGridViewCellStyle3.Font = new System.Drawing.Font("宋体", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            dataGridViewCellStyle3.ForeColor = System.Drawing.SystemColors.WindowText;
            dataGridViewCellStyle3.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            dataGridViewCellStyle3.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle3.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
            this.dataGridView.ColumnHeadersDefaultCellStyle = dataGridViewCellStyle3;
            this.dataGridView.ColumnHeadersHeight = 26;
            this.dataGridView.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.dgvtbcAddress,
            this.dgvtbcExamCode,
            this.dgvtbcCourseId,
            this.dgvtbcNo,
            this.dgvtbcSemester,
            this.dgvtbcCourse,
            this.dgvtbcGrade,
            this.dgvtbcProgress,
            this.dgvtbcScore,
            this.dgvtbcLearnTime,
            this.dgvlcOperation,
            this.dgvlcData});
            this.dataGridView.Location = new System.Drawing.Point(12, 45);
            this.dataGridView.MultiSelect = false;
            this.dataGridView.Name = "dataGridView";
            this.dataGridView.ReadOnly = true;
            this.dataGridView.RowHeadersVisible = false;
            this.dataGridView.RowTemplate.Height = 23;
            this.dataGridView.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.dataGridView.Size = new System.Drawing.Size(1057, 391);
            this.dataGridView.TabIndex = 1;
            this.dataGridView.CellContentClick += new System.Windows.Forms.DataGridViewCellEventHandler(this.dataGridView1_CellContentClick);
            // 
            // dgvtbcAddress
            // 
            this.dgvtbcAddress.DataPropertyName = "Address";
            this.dgvtbcAddress.HeaderText = "Address";
            this.dgvtbcAddress.Name = "dgvtbcAddress";
            this.dgvtbcAddress.ReadOnly = true;
            this.dgvtbcAddress.Visible = false;
            // 
            // dgvtbcExamCode
            // 
            this.dgvtbcExamCode.DataPropertyName = "ExamCode";
            this.dgvtbcExamCode.HeaderText = "ExamCode";
            this.dgvtbcExamCode.Name = "dgvtbcExamCode";
            this.dgvtbcExamCode.ReadOnly = true;
            this.dgvtbcExamCode.Visible = false;
            // 
            // dgvtbcCourseId
            // 
            this.dgvtbcCourseId.DataPropertyName = "CourseId";
            this.dgvtbcCourseId.HeaderText = "CourseId";
            this.dgvtbcCourseId.Name = "dgvtbcCourseId";
            this.dgvtbcCourseId.ReadOnly = true;
            this.dgvtbcCourseId.Visible = false;
            // 
            // dgvtbcNo
            // 
            this.dgvtbcNo.DataPropertyName = "No";
            this.dgvtbcNo.FillWeight = 4F;
            this.dgvtbcNo.HeaderText = "序号";
            this.dgvtbcNo.Name = "dgvtbcNo";
            this.dgvtbcNo.ReadOnly = true;
            // 
            // dgvtbcSemester
            // 
            this.dgvtbcSemester.DataPropertyName = "Semester";
            this.dgvtbcSemester.FillWeight = 6F;
            this.dgvtbcSemester.HeaderText = "学期";
            this.dgvtbcSemester.Name = "dgvtbcSemester";
            this.dgvtbcSemester.ReadOnly = true;
            // 
            // dgvtbcCourse
            // 
            this.dgvtbcCourse.DataPropertyName = "Course";
            this.dgvtbcCourse.FillWeight = 20F;
            this.dgvtbcCourse.HeaderText = "课程";
            this.dgvtbcCourse.Name = "dgvtbcCourse";
            this.dgvtbcCourse.ReadOnly = true;
            // 
            // dgvtbcGrade
            // 
            this.dgvtbcGrade.DataPropertyName = "Grade";
            this.dgvtbcGrade.FillWeight = 18F;
            this.dgvtbcGrade.HeaderText = "成绩明细";
            this.dgvtbcGrade.Name = "dgvtbcGrade";
            this.dgvtbcGrade.ReadOnly = true;
            // 
            // dgvtbcProgress
            // 
            this.dgvtbcProgress.DataPropertyName = "Progress";
            this.dgvtbcProgress.FillWeight = 10F;
            this.dgvtbcProgress.HeaderText = "学习进度";
            this.dgvtbcProgress.Name = "dgvtbcProgress";
            this.dgvtbcProgress.ReadOnly = true;
            // 
            // dgvtbcScore
            // 
            this.dgvtbcScore.DataPropertyName = "Score";
            this.dgvtbcScore.FillWeight = 14F;
            this.dgvtbcScore.HeaderText = "学习成绩";
            this.dgvtbcScore.Name = "dgvtbcScore";
            this.dgvtbcScore.ReadOnly = true;
            // 
            // dgvtbcLearnTime
            // 
            this.dgvtbcLearnTime.DataPropertyName = "LearnTime";
            this.dgvtbcLearnTime.FillWeight = 12F;
            this.dgvtbcLearnTime.HeaderText = "学习时间";
            this.dgvtbcLearnTime.Name = "dgvtbcLearnTime";
            this.dgvtbcLearnTime.ReadOnly = true;
            // 
            // dgvlcOperation
            // 
            this.dgvlcOperation.DataPropertyName = "Operation";
            this.dgvlcOperation.FillWeight = 7F;
            this.dgvlcOperation.HeaderText = "操作";
            this.dgvlcOperation.LinkBehavior = System.Windows.Forms.LinkBehavior.AlwaysUnderline;
            this.dgvlcOperation.Name = "dgvlcOperation";
            this.dgvlcOperation.ReadOnly = true;
            this.dgvlcOperation.Resizable = System.Windows.Forms.DataGridViewTriState.True;
            this.dgvlcOperation.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.Automatic;
            // 
            // dgvlcData
            // 
            this.dgvlcData.DataPropertyName = "Data";
            this.dgvlcData.FillWeight = 7F;
            this.dgvlcData.HeaderText = "资料";
            this.dgvlcData.LinkBehavior = System.Windows.Forms.LinkBehavior.AlwaysUnderline;
            this.dgvlcData.Name = "dgvlcData";
            this.dgvlcData.ReadOnly = true;
            this.dgvlcData.Resizable = System.Windows.Forms.DataGridViewTriState.True;
            this.dgvlcData.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.Automatic;
            // 
            // ckRemember
            // 
            this.ckRemember.AutoSize = true;
            this.ckRemember.Location = new System.Drawing.Point(439, 19);
            this.ckRemember.Name = "ckRemember";
            this.ckRemember.Size = new System.Drawing.Size(48, 16);
            this.ckRemember.TabIndex = 27;
            this.ckRemember.Text = "记住";
            this.ckRemember.UseVisualStyleBackColor = true;
            // 
            // lvLog
            // 
            this.lvLog.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.lvLog.DrawMode = System.Windows.Forms.DrawMode.OwnerDrawVariable;
            this.lvLog.Font = new System.Drawing.Font("Tahoma", 9F);
            this.lvLog.FormattingEnabled = true;
            this.lvLog.ItemHeight = 14;
            this.lvLog.Location = new System.Drawing.Point(12, 442);
            this.lvLog.Name = "lvLog";
            this.lvLog.Size = new System.Drawing.Size(1057, 158);
            this.lvLog.TabIndex = 33;
            this.lvLog.DrawItem += new System.Windows.Forms.DrawItemEventHandler(this.lvLog_DrawItem);
            this.lvLog.MeasureItem += new System.Windows.Forms.MeasureItemEventHandler(this.lvLog_MeasureItem);
            // 
            // lblLoginInfo
            // 
            this.lblLoginInfo.AutoSize = true;
            this.lblLoginInfo.Location = new System.Drawing.Point(612, 21);
            this.lblLoginInfo.Name = "lblLoginInfo";
            this.lblLoginInfo.Size = new System.Drawing.Size(53, 12);
            this.lblLoginInfo.TabIndex = 35;
            this.lblLoginInfo.Text = "ShowInfo";
            // 
            // saveFileDialog1
            // 
            this.saveFileDialog1.Title = "下载资料";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.ForeColor = System.Drawing.Color.Red;
            this.label1.Location = new System.Drawing.Point(12, 606);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(641, 12);
            this.label1.TabIndex = 36;
            this.label1.Text = "本软件不得用于商业用途，仅做学习交流，请下载后24小时内删除，如有违反触犯任何法律及相关条文，本人概不负责！";
            // 
            // linkLabel1
            // 
            this.linkLabel1.AutoSize = true;
            this.linkLabel1.Location = new System.Drawing.Point(992, 606);
            this.linkLabel1.Name = "linkLabel1";
            this.linkLabel1.Size = new System.Drawing.Size(77, 12);
            this.linkLabel1.TabIndex = 37;
            this.linkLabel1.TabStop = true;
            this.linkLabel1.Text = "点我查看源码";
            this.linkLabel1.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.linkLabel1_LinkClicked);
            // 
            // MainForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1081, 625);
            this.Controls.Add(this.linkLabel1);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.lblLoginInfo);
            this.Controls.Add(this.lvLog);
            this.Controls.Add(this.dataGridView);
            this.Controls.Add(this.ckRemember);
            this.Controls.Add(this.txtPassWord);
            this.Controls.Add(this.txtUserName);
            this.Controls.Add(this.lblPwd);
            this.Controls.Add(this.lblUserName);
            this.Controls.Add(this.btnLogin);
            this.Name = "MainForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "电子科大学生管理平台-辅助工具 By.月影银翔（有问题邮件tanxiang6985@foxmail.com） v";
            this.Shown += new System.EventHandler(this.MainForm_Shown);
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private Button btnLogin;
        private Label lblUserName;
        private Label lblPwd;
        private TextBox txtUserName;
        private TextBox txtPassWord;
        private DataGridView dataGridView;
        private CheckBox ckRemember;
        private ListBox lvLog;
        private Label lblLoginInfo;
        private SaveFileDialog saveFileDialog1;
        private DataGridViewTextBoxColumn dgvtbcAddress;
        private DataGridViewTextBoxColumn dgvtbcExamCode;
        private DataGridViewTextBoxColumn dgvtbcCourseId;
        private DataGridViewTextBoxColumn dgvtbcNo;
        private DataGridViewTextBoxColumn dgvtbcSemester;
        private DataGridViewTextBoxColumn dgvtbcCourse;
        private DataGridViewTextBoxColumn dgvtbcGrade;
        private DataGridViewTextBoxColumn dgvtbcProgress;
        private DataGridViewTextBoxColumn dgvtbcScore;
        private DataGridViewTextBoxColumn dgvtbcLearnTime;
        private DataGridViewLinkColumn dgvlcOperation;
        private DataGridViewLinkColumn dgvlcData;
        private Label label1;
        private LinkLabel linkLabel1;
    }
}

