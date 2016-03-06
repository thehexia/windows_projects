using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Diagnostics;
using System.Reflection;
using System.Collections;

namespace CalcStudio
{
    public enum CalcKind
    {
        simpleCalc = 1,
        complexCalc = 2
    }

    public class CalcCheckBox : CheckBox
    {
        public CalcCheckBox(string text, MethodInfo method)
            : base()
        {
            base.Text = text;
            this.method = method;
        }

        public MethodInfo method { get; set; }

        private void InitializeComponent()
        {
            this.SuspendLayout();
            this.ResumeLayout(false);
        }
    }

    public class SimpleCalcButton : Button
    {
        public SimpleCalcButton(MethodInfo method)
        {
            this.method = method;
        }

        public MethodInfo method { get; set; }
        // Maintain text box inputs.
        public TextBox int1 { get; set; }
        public TextBox int2 { get; set; }

        // maintain ref to result text box
        public TextBox result { get; set; }
    }

    public class ComplexCalcButton : Button
    {
        public ComplexCalcButton(MethodInfo method)
        {
            this.method = method;
        }

        public MethodInfo method { get; set; }
        // Maintain text box inputs.
        public TextBox real1 { get; set; }
        public TextBox img1 { get; set; }
        public TextBox real2 { get; set; }
        public TextBox img2 { get; set; }

        // maintain ref to result text box
        public TextBox result { get; set; }
    }

    public class CalcPerformEventArgs : EventArgs
    {
        public MethodInfo method { get; set; }
    }

    public class GroupMethods
    {
        public GroupMethods()
        {
            methods = new List<CalcCheckBox>();
            type = null;
        }

        public GroupMethods(Type t, CalcKind k)
        {
            methods = new List<CalcCheckBox>();
            type = t;
            kind = k;
        }

        public Type type { get; set; }
        public CalcKind kind { get; set; }
        public List<CalcCheckBox> methods { get; set; }
    }
}
