using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WindowsFormsApp1
{
    [Serializable]
    class Student
    {
        public String ID { get; set; }
        public string Name { get; set; }
        public string Address { get; set; }
        public string Awards { get; set; }
        public bool Sex { get; set; }

        public string Dept { get; set; }

        public string College { get; set; }

        public Student(string id,string name,string address,string awards,bool sex,string college, string dept)
        {
            ID = id;
            Name = name;
            Address = address;
            Awards = awards;
            Sex = sex;
            Dept = dept;
            College = college;
        }
    }
}
