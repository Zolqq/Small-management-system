using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace WindowsFormsApp1
{

    [Serializable]
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            Deserialize();
        }


        /// <summary>
        /// 清空设置
        /// </summary>
        private void EmptyContent()
        {
            textBox1.Text = "";
            textBox1.Enabled = true;
            textBox2.Text = "";
            textBox3.Text = "";
            textBox4.Text = "";
            textBox5.Text = "";
            radioButton1.Checked = true;
            errorProvider1.Clear();
        }

        /// <summary>
        /// 添加新生
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void button1_Click(object sender, EventArgs e)
        {
            if (!ValidateChildren())
            {
                MessageBox.Show("还有信息未通过验证！");
            }
            else
            {
                TreeNode node = new TreeNode(textBox2.Text);
                bool sex = radioButton1.Checked ? true : false;
                Student ss = new Student(textBox1.Text, textBox2.Text, textBox3.Text, textBox4.Text, sex, comboBox1.Text, comboBox2.Text);
                node.Tag = ss;
                SelectedNodeChanged();
                if (treeView1.SelectedNode.Level == 2)
                {
                    treeView1.SelectedNode.Nodes.Add(node);
                    treeView1.SelectedNode.Expand();
                    EmptyContent();
                }
                else
                {
                    MessageBox.Show("必须选中系节点！");
                }
            }
        }


        /// <summary>
        /// 更改学生信息
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void button2_Click(object sender, EventArgs e)
        {
            Student ss = treeView1.SelectedNode.Tag as Student;
            if (!ValidateChildren())
            {
                MessageBox.Show("还有信息未通过验证！");
            }
            else
            {
                if (ss.ID == textBox1.Text.Trim())
                {
                    ss.Name = textBox2.Text.Trim();
                    ss.Address = textBox3.Text.Trim();
                    ss.Awards = textBox4.Text.Trim();
                    ss.Sex = radioButton1.Checked;
                    treeView1.SelectedNode.Text = ss.Name;
                    if (comboBox1.Text.Trim() != ss.College || comboBox2.Text.Trim() != ss.Dept)
                    {
                        ss.College = comboBox1.Text.Trim();
                        ss.Dept = comboBox2.Text.Trim();
                        TreeNode node = treeView1.SelectedNode;
                        SelectedNodeChanged();
                        treeView1.Nodes.Remove(node);
                        treeView1.SelectedNode.Nodes.Add(node);
                        treeView1.SelectedNode = node;
                        treeView1.SelectedNode.Expand();
                       
                    }
                    MessageBox.Show("修改成功！", "提示");
                }
            }
        }

        /// <summary>
        /// 删除学生
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void button3_Click(object sender, EventArgs e)
        {
            if (treeView1.SelectedNode.Level == 3)
            {
                if (MessageBox.Show("是否删除此学生？", "提示", MessageBoxButtons.YesNo) == DialogResult.Yes)
                {
                    treeView1.Nodes.Remove(treeView1.SelectedNode);
                }
            }
        }

        /// <summary>
        /// 查找学生
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void button5_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(textBox6.Text.Trim()))
            {
                MessageBox.Show("请输入学生学号", "提示");
                return;
            }

            foreach (TreeNode college in treeView1.Nodes[0].Nodes)
            {
                foreach (TreeNode dept in college.Nodes)
                {
                    foreach (TreeNode stu in dept.Nodes)
                    {
                        if (stu.Tag != null)
                        {
                            Student ss = stu.Tag as Student;
                            if (ss.ID == textBox6.Text.Trim())
                            {
                                treeView1.SelectedNode = stu;
                                treeView1.SelectedNode.Expand();
                                textBox6.Text = "";
                                return;
                            }
                        }
                    }
                }
            }
            MessageBox.Show("该学生不存在", "提示");
        }

        /// <summary>
        /// 信息验证
        /// </summary>
        /// <returns></returns>
        public override bool ValidateChildren()
        {
            errorProvider1.Clear();
            bool flag = true;
            string pattern = "^[0-9]{1,}$";
            System.Text.RegularExpressions.Regex reg = new System.Text.RegularExpressions.Regex(pattern);

            if (string.IsNullOrEmpty(textBox1.Text.Trim()))
            {
                errorProvider1.SetError(textBox1, "学号不能为空");
                flag = false;
            }
            else if (!reg.IsMatch(textBox1.Text.Trim()))
            {
                errorProvider1.SetError(textBox1, "学号只能为纯数字");
                flag = false;
            }


            if (string.IsNullOrEmpty(textBox2.Text.Trim()))
            {
                errorProvider1.SetError(textBox2, "姓名不能为空");
                flag = false;
            }
            if (string.IsNullOrEmpty(textBox3.Text.Trim()))
            {
                errorProvider1.SetError(textBox3, "住址不能为空");
                flag = false;
            }
            if (string.IsNullOrEmpty(comboBox1.Text.Trim()))
            {
                errorProvider1.SetError(comboBox1, "学院不能为空");
                flag = false;
            }
            if (string.IsNullOrEmpty(comboBox2.Text.Trim()))
            {
                errorProvider1.SetError(comboBox2, "系不能为空");
                flag = false;
            }
            return flag;

        }

        /// <summary>
        /// 单独对学号是否重复验证
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void textBox1_Validating(object sender, CancelEventArgs e)
        {
            if (HasSameID(textBox1.Text.Trim()))
            {
                errorProvider1.SetError(textBox1, "学号已存在");
                button1.Enabled = false;
            }
            else
            {
                errorProvider1.SetError(textBox1, "");
                button1.Enabled = true;
            }
        }

        /// <summary>
        /// 检查当前学号是否重复
        /// </summary>
        /// <param name="text"></param>
        /// <returns></returns>
        private bool HasSameID(string text)
        {
            foreach (TreeNode college in treeView1.Nodes[0].Nodes)
            {
                foreach (TreeNode dept in college.Nodes)
                {
                    foreach (TreeNode stu in dept.Nodes)
                    {
                        if (stu.Tag != null)
                        {
                            Student ss = stu.Tag as Student;
                            if (ss != null)
                            {
                                if (ss.ID == text)
                                {
                                    return true;
                                }
                            }
                        }
                    }
                }
            }
            return false;
        }

        /// <summary>
        /// 添加系
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void button4_Click(object sender, EventArgs e)
        {
            if (treeView1.SelectedNode.Level == 1)
            {
                if (string.IsNullOrEmpty(textBox5.Text.Trim()))
                {
                    MessageBox.Show("请输入系名称", "提示");
                    return;
                }
                
                foreach (TreeNode college in treeView1.Nodes[0].Nodes)
                {
                    foreach (TreeNode dept in college.Nodes)
                    {
                        if (dept.Text == textBox5.Text.Trim())
                        {
                            MessageBox.Show("不能重复添加同名院系");
                            textBox5.Text = "";
                            return;
                        }
                    }
                }

                TreeNode node = new TreeNode(textBox5.Text.Trim());
                treeView1.SelectedNode.Nodes.Add(node);
                treeView1.SelectedNode.Expand();
                textBox5.Text = "";
                EmptyContent();
            }
            else
            {
                MessageBox.Show("必须选中院节点！");
            }
        }


        /// <summary>
        /// 添加院
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void button6_Click(object sender, EventArgs e)
        {
            if (treeView1.SelectedNode.Level == 0)
            {
                if (string.IsNullOrEmpty(textBox7.Text.Trim()))
                {
                    MessageBox.Show("请输入院名称", "提示");
                    return;
                }
                foreach (TreeNode college in treeView1.SelectedNode.Nodes)
                {
                    if (college.Text == textBox7.Text.Trim())
                    {
                        MessageBox.Show("不能重复添加同名学院");
                        textBox7.Text = "";
                        return;
                    }
                }
                TreeNode node = new TreeNode(textBox7.Text.Trim());
                treeView1.SelectedNode.Nodes.Add(node);
                treeView1.SelectedNode.Expand();
                treeView1.SelectedNode = node;
                textBox7.Text = "";
                EmptyContent();
            }
            else
            {
                MessageBox.Show("必须选中绍兴文理学院节点！");
            }
        }

        private void treeView1_AfterSelect(object sender, TreeViewEventArgs e)
        {
            EmptyContent();
            if (e.Node.Level == 0)
            {
                button6.Enabled = true;
                comboBox1.Items.Clear();
                foreach (TreeNode college in treeView1.Nodes[0].Nodes)
                {
                    comboBox1.Items.Add(college.Text);
                }
                comboBox2.Items.Clear();
            }
            else if (e.Node.Level == 1)//表示学院
            {
                button4.Enabled = true;
                comboBox1.Text = e.Node.Text;
                comboBox2.Text = "";
            }
            else if (e.Node.Level == 2)//表明是系
            {
                comboBox1.Text = e.Node.Parent.Text;
                comboBox2.Text = e.Node.Text;
            }
            else if (e.Node.Level == 3)//表明是学生
            {
                textBox1.Enabled = false;
                Student ss = e.Node.Tag as Student;
                if (ss == null) return;
                textBox1.Text = ss.ID;
                textBox2.Text = ss.Name;
                textBox3.Text = ss.Address;
                textBox4.Text = ss.Awards;
                if (ss.Sex)
                    radioButton1.Checked = true;
                else
                    radioButton2.Checked = true;
                comboBox1.Text = ss.College;
                comboBox2.Text = ss.Dept;
                button2.Enabled = true;
                button3.Enabled = true;
                button1.Enabled = false;
            }


            if (e.Node.Level != 3)
            {
                button2.Enabled = false;
                button3.Enabled = false;
                button1.Enabled = true;
            }
            if (e.Node.Level != 1)
            {
                button4.Enabled = false;
            }
            if (e.Node.Level != 0)
            {
                button6.Enabled = false;
            }

        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            Serialize();
        }

        /// <summary>
        /// 序列化
        /// </summary>
        private void Serialize()
        {
            FileStream fs = new FileStream("DataFile.dat", FileMode.Create);
            BinaryFormatter formatter = new BinaryFormatter();
            try
            {
                formatter.Serialize(fs, treeView1.Nodes[0]);
            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message, "错误信息", MessageBoxButtons.OK);
            }
            finally
            {
                fs.Close();
            }
        }

        /// <summary>
        /// 反序列化
        /// </summary>
        private void Deserialize()
        {
            TreeNode node = null;
            FileStream fs = null;
            try
            {
                fs = new FileStream("DataFile.dat", FileMode.Open);
                BinaryFormatter formatter = new BinaryFormatter();
                if (formatter == null) return;
                node = (TreeNode)formatter.Deserialize(fs);
            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message, "错误信息", MessageBoxButtons.OK);
            }
            finally
            {
                fs.Close();
            }

            if (node != null)
            {
                treeView1.Nodes.Clear();
                treeView1.Nodes.Add(node);
            }

        }

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            comboBox2.Items.Clear();
            if (comboBox1.SelectedItem!= null)
            {
                foreach (TreeNode college in treeView1.Nodes[0].Nodes)
                {
                    if (college.Text == comboBox1.SelectedItem.ToString())
                    {
                        foreach (TreeNode dept in college.Nodes)
                        {
                            comboBox2.Items.Add(dept.Text);
                        }
                    }
                }
            }   
        }

        private void SelectedNodeChanged()
        {
            foreach (TreeNode college in treeView1.Nodes[0].Nodes)
            {
                if (college.Text == comboBox1.SelectedItem.ToString())
                {
                    foreach (TreeNode dept in college.Nodes)
                    {
                        if (dept.Text == comboBox2.Text)
                        {
                            treeView1.SelectedNode = dept;
                        }
                    }
                }
            }
        }


        private void treeView1_DragEnter(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(typeof(TreeNode)))
            {
                e.Effect = DragDropEffects.Move;
            }
            else
            {
                e.Effect = DragDropEffects.None;
            }

        }

        private void treeView1_DragDrop(object sender, DragEventArgs e)
        {
            TreeNode selectNode = (TreeNode)e.Data.GetData(typeof(TreeNode));//源端数据
            //1.根据鼠标坐标获得目标节点
            Point pt = ((TreeView)sender).PointToClient(new Point(e.X, e.Y));
            TreeNode DestinationNode = ((TreeView)sender).GetNodeAt(pt);

            if (DestinationNode != null && !DestinationNode.Equals(selectNode) && DestinationNode.Level + 1 == selectNode.Level)//目标节点不为空、目标节点不是源节点
            {
                if (selectNode.Level == 2)
                {
                    foreach(TreeNode student in selectNode.Nodes)
                    {
                        Student ss = student.Tag as Student;
                        ss.College = DestinationNode.Text;
                    }
                }
                else if (selectNode.Level == 3)
                {
                    Student ss = selectNode.Tag as Student;
                    ss.College = DestinationNode.Parent.Text;
                    ss.Dept = DestinationNode.Text;
                }
                selectNode.Remove();
                DestinationNode.Nodes.Add(selectNode);
                DestinationNode.Expand();
                treeView1.SelectedNode = selectNode;
            }
            else
            {
                MessageBox.Show("请拖放至正确的节点", "提示");
                return;
            }


        }

        private void treeView1_ItemDrag(object sender, ItemDragEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                if (((TreeNode)e.Item).Level > 0)
                {
                    treeView1.DoDragDrop(e.Item, DragDropEffects.Copy | DragDropEffects.Move);
                }
            }
        }

        private void tabControl1_SelectedIndexChanged(object sender, EventArgs e)
        {
            treeView1.SelectedNode = treeView1.Nodes[0];
        }
    }
}
