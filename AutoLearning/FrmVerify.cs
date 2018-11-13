using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace AutoLearning
{
    /// <summary>
    /// 文 件 名：FrmVerify
    /// 创建人员：谭翔
    /// 创建时间：2018/11/8 17:52:15
    /// 说明描述：
    /// 修改记录：
    /// <para>R1、……</para>
    /// <para>R2、……</para>
    /// </summary>
    public partial class FrmVerify : Form
    {
        public FrmVerify()
        {
            InitializeComponent();
        }

        public void SetImage(Image img)
        {
            pictureBox1.Image = img;
        }

        private void textBox1_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                DialogResult = DialogResult.OK;
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.OK;
        }

        public string GetInputVerify()
        {
            return textBox1.Text.Trim();
        }
    }
}
