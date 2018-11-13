namespace AutoLearning
{
    /// <summary>
    /// 文 件 名：Lesson
    /// 创建人员：谭翔
    /// 创建时间：2018/10/20 17:52:19
    /// 说明描述：
    /// 修改记录：
    /// <para>R1、……</para>
    /// <para>R2、……</para>
    /// </summary>
    public class Lesson
    {
        public string ContentId { get; set; }

        public string IsPreTest { get; set; }

        public string PreTestId { get; set; }

        public string CourseId { get; set; }

        public string ContentType { get; set; }

        public string RelationId { get; set; }

        public string ParentId { get; set; }

        public string Status { get; set; }

        public string SubjectCount { get; set; }

        public string Sequence { get; set; }

        public string RowIndex { get; set; }

        public string RecordCount { get; set; }
        public string Name { get; set; }
    }
}
