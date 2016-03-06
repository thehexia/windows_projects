using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Diagnostics;
using System.Reflection;
using System.Collections;
using System.Numerics;

namespace CalcStudio
{
    public partial class CalcForm : Form
    {
        private Assembly asm;
        private List<GroupMethods> groups;

        public CalcForm()
        {
            InitializeComponent();
            this.Text = "Calculator Studio";
            this.tableLayoutPanel1.Dock = DockStyle.Top;
            makeButton.Click += makeButton_Click;
            asm = null;
            groups = new List<GroupMethods>();
        }

        private void fileToolStripMenuItem_Click(object sender, EventArgs e)
        {

        }

        private void calcPerform_Click(object sender, EventArgs args)
        {
            if (sender is SimpleCalcButton)
            {
                SimpleCalcButton but = (SimpleCalcButton)sender;
                // get result box
                TextBox resbox = but.result;

                // Get the class that declared this type.
                Type calc = but.method.DeclaringType;
                Object obj = Activator.CreateInstance(calc);

                // Get the input
                // Try to parse a mode the user input.
                // If failed set to 0
                int a;
                if (!int.TryParse(but.int1.Text, out a))
                    a = 0;

                int b;
                if (!int.TryParse(but.int2.Text, out b))
                    b = 0;

                // Invoke the method.
                var result = but.method.Invoke(obj, new object[] { a, b });
                // Write the result
                resbox.Text = "Result: " + result;
            }
            else if (sender is ComplexCalcButton)
            {
                ComplexCalcButton but = (ComplexCalcButton)sender;
                // get result box
                TextBox resbox = but.result;

                // Get the class that declared this type.
                Type calc = but.method.DeclaringType;
                Object obj = Activator.CreateInstance(calc);

                // Get the input
                // Try to parse a mode the user input.
                // If failed set to 0
                double r1;
                double i1;
                if (!double.TryParse(but.real1.Text, out r1))
                    r1 = 0;

                if (!double.TryParse(but.img1.Text, out i1))
                    i1 = 0;

                double r2;
                double i2;
                if (!double.TryParse(but.real2.Text, out r2))
                    r2 = 0;

                if (!double.TryParse(but.img2.Text, out i2))
                    r1 = 0;

                // Invoke the method.
                Complex result = (Complex) but.method.Invoke(obj, new object[] { new Complex(r1, i1), new Complex(r2, i2) });
                // Write the result
                resbox.Text = "Result: " + result.Real + "+" + result.Imaginary + "i";
            }
        }

        // Causes calculator to be made in a new form.
        private void makeButton_Click(object sender, EventArgs e)
        {
            Calculator form = new Calculator();

            // create a table layout for the whole thing.
            TableLayoutPanel panel = new TableLayoutPanel();
            panel.Dock = DockStyle.Top;
            panel.AutoSize = true;
            form.Controls.Add(panel);

            // make the result box.
            TextBox res = new TextBox();
            res.ReadOnly = true;

            foreach (var group in groups)
            {
                // Create a group box for the type.
                GroupBox gbox = new GroupBox();
                gbox.Text = group.type.Name;
                gbox.Dock = DockStyle.Top;
                gbox.AutoSize = true;
                bool addBox = false;

                // Add a table panel inside the box.
                TableLayoutPanel optPanel = new TableLayoutPanel();
                optPanel.AutoSize = true;
                optPanel.Dock = DockStyle.Top;

                // Add the text boxes for input.
                if (group.kind == CalcKind.simpleCalc)
                {
                    optPanel.ColumnCount = 2;

                    Label label1 = new Label();
                    label1.Text = "param 1:";
                    TextBox box1 = new TextBox();
                    optPanel.Controls.Add(label1);
                    optPanel.Controls.Add(box1);

                    Label label2 = new Label();
                    label2.Text = "param 2:";
                    TextBox box2 = new TextBox();
                    optPanel.Controls.Add(label2);
                    optPanel.Controls.Add(box2);

                    // Iterate over the check boxes and check if any are checked.
                    foreach (var opt in group.methods)
                    {
                        if (opt.Checked)
                        {
                            addBox = true;
                            SimpleCalcButton button = new SimpleCalcButton(opt.method);
                            button.Text = opt.method.Name;
                            button.Click += calcPerform_Click;
                            // Add as props to the form for lazy access.
                            button.int1 = box1;
                            button.int2 = box2;
                            button.result = res;
                            optPanel.Controls.Add(button);
                        }
                    }

                    if (addBox)
                    {
                        gbox.Controls.Add(optPanel);
                        panel.Controls.Add(gbox);
                    }
                }

                // Add the text boxes for input.
                if (group.kind == CalcKind.complexCalc)
                {
                    optPanel.ColumnCount = 5;
                    // + sign
                    Label p1 = new Label();
                    p1.Text = " + ";
                    p1.AutoSize = true;
                    // i sumbol
                    Label i = new Label();
                    i.Text = " i";
                    i.AutoSize = true;

                    // + sign
                    Label p2 = new Label();
                    p2.Text = " + ";
                    p2.AutoSize = true;
                    // i sumbol
                    Label i2 = new Label();
                    i2.Text = " i";
                    i2.AutoSize = true;


                    Label parm1 = new Label();
                    parm1.Text = "param 1:";
                    TextBox realBox1 = new TextBox();
                    TextBox imgBox1 = new TextBox();
                    // Add param 1
                    optPanel.Controls.Add(parm1);
                    optPanel.Controls.Add(realBox1);
                    optPanel.Controls.Add(p1);
                    optPanel.Controls.Add(imgBox1);
                    optPanel.Controls.Add(i);

                    Label parm2 = new Label();
                    parm2.Text = "param 2:";
                    TextBox realBox2 = new TextBox();
                    TextBox imgBox2 = new TextBox();

                    // Add param 2
                    optPanel.Controls.Add(parm2);
                    optPanel.Controls.Add(realBox2);
                    optPanel.Controls.Add(p2);
                    optPanel.Controls.Add(imgBox2);
                    optPanel.Controls.Add(i2);

                    // Iterate over the check boxes and check if any are checked.
                    foreach (var opt in group.methods)
                    {
                        if (opt.Checked)
                        {
                            addBox = true;
                            ComplexCalcButton button = new ComplexCalcButton(opt.method);
                            button.Text = opt.method.Name;
                            button.Click += calcPerform_Click;
                            // Add as props to the form for lazy access.
                            button.real1 = realBox1;
                            button.img1 = imgBox1;
                            button.real2 = realBox2;
                            button.img2 = imgBox2;
                            button.result = res;
                            optPanel.Controls.Add(button);
                        }
                    }

                    if (addBox)
                    {
                        gbox.Controls.Add(optPanel);
                        panel.Controls.Add(gbox);
                    }
                }
            }

            // Add result text box
            panel.Controls.Add(res);

            form.Show();
        }

        private void openFileDialog1_FileOk(object sender, CancelEventArgs e)
        {
            this.Activate();
            try
            {
                asm = Assembly.LoadFrom(openFileDialog1.FileName);
                // Read through the dll and save all of its method info
                foreach (var type in asm.GetTypes())
                {
                    // Add a group box for the type.
                    GroupBox gbox = new GroupBox();
                    gbox.Text = type.Name;
                    gbox.Dock = DockStyle.Top;
                    gbox.AutoSize = true;

                    // Add a table panel inside the box.
                    TableLayoutPanel panel = new TableLayoutPanel();
                    panel.Dock = DockStyle.Top;
                    gbox.Controls.Add(panel);

                    // Add the group box to the form.
                    tableLayoutPanel1.Controls.Add(gbox);

                    // Removes object inherited functions.
                    // Source: http://stackoverflow.com/questions/12475417/dont-return-tostring-equals-gethashcode-gettype-with-type-getmethods
                    var methods = type.GetMethods()
                                        .Where(m => m.DeclaringType != typeof(object));

                    // The kind of calculator.
                    CalcKind kind = 0;
                    if (type == asm.GetType("Calc.SimpleCalc"))
                        kind = CalcKind.simpleCalc;
                    else
                        kind = CalcKind.complexCalc;

                    // Defined in CalcControls.cs. Used to keep track of checkboxes in their respective groups.
                    GroupMethods group = new GroupMethods(type, kind);

                    foreach (var method in methods)
                    {
                        CalcCheckBox cbox = new CalcCheckBox(method.ToString(), method);
                        cbox.AutoSize = true;
                        panel.Controls.Add(cbox);
                        group.methods.Add(cbox);
                    }

                    groups.Add(group);
                }

                this.Invalidate();
            }
            catch (Exception except)
            {
                Debug.WriteLine(except.ToString());
            }
        }

        private void openToolStripMenuItem_Click(object sender, EventArgs e)
        {
            // Set filter options and filter index.
            openFileDialog1.Filter = "DLL Files (.dll)|*.dll|All Files (*.*)|*.*";
            openFileDialog1.FilterIndex = 1;

            openFileDialog1.Multiselect = false;

            // Call the ShowDialog method to show the dialog box.
            openFileDialog1.ShowDialog();
        }
    }
}
