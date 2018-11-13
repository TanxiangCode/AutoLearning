using System;
using System.Collections.Generic;
using System.Net;
using System.Text;

namespace AutoLearning
{
    /// <summary>
    /// 文 件 名：LearnData
    /// 创建人员：谭翔
    /// 创建时间：2018/10/20 17:50:36
    /// 说明描述：
    /// 修改记录：
    /// <para>R1、……</para>
    /// <para>R2、……</para>
    /// </summary>
    public class LearnData
    {
        /// <summary>
        /// 序号
        /// </summary>
        public string No { get; set; }

        /// <summary>
        /// 编号
        /// </summary>
        public string CourseId { get; set; }

        /// <summary>
        /// 课程
        /// </summary>
        public string Course { get; set; }

        /// <summary>
        /// 学期
        /// </summary>
        public string Semester { get; set; }

        /// <summary>
        /// 进度
        /// </summary>
        public string Progress { get; set; }

        /// <summary>
        /// 成绩
        /// </summary>
        public string Score { get; set; }

        /// <summary>
        /// 学习时间
        /// </summary>
        public string LearnTime { get; set; }

        /// <summary>
        /// 操作
        /// </summary>
        public string Operation { get; set; }

        /// <summary>
        /// 数据
        /// </summary>
        public string Data { get; set; }

        /// <summary>
        /// 考试代码
        /// </summary>
        public string ExamCode { get; set; }

        /// <summary>
        /// 资料
        /// </summary>
        public string Address { get; set; }

        /// <summary>
        /// 成绩明细
        /// </summary>
        public string Grade { get; set; }

        /// <summary>
        /// 课程
        /// </summary>
        public List<Lesson> Lessons { get; set; }
    }
}
