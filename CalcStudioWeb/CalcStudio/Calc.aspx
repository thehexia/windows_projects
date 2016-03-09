<%@ Page Language="C#" %>
<%@ Assembly Name="WPCalc" %>
<%@ Import Namespace="System.Reflection" %>
<%@ Import Namespace="System.Collections.Generic" %>
<%@ Import Namespace="System.Linq" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title></title>
</head>
<body>
    <form id="calcPick" action="Calc.aspx" runat="server">
        <asp:Table ID="studioTable" runat="server"></asp:Table>
        <asp:Table ID="optTable" BorderWidth="1" BorderStyle="Solid" GridLines="Both" runat="server"></asp:Table>
        <span><asp:Button ID="createCalc" runat="server" Text="Create Calculator" /></span>
    </form>
    <br />
    <br />
    <h3 style="color:red"><asp:Label ID="errorBox" RunAt="server" /></h3>
    <br />
    <br />
    <h3><asp:Label ID="answerBox" runat="server"></asp:Label></h3>
</body>
</html>

<script language="C#" runat="server">

    public enum CalcKind
    {
        ComplexCalc,
        SimpleCalc
    };

    // Store metadata about a specific calculator.
    public class CalculatorInfo
    {
        public CalculatorInfo()
        {
            Methods = new List<MethodInfo>();
            Checkboxes = new List<CheckBox>();
        }

        public List<MethodInfo> Methods { get; set; }
        public List<CheckBox> Checkboxes { get; set; }
        public Panel ControlPanel { get; set; }
        public Panel InputPanel { get; set; }
        public CalcKind Kind { get; set; }
    }

    private Assembly asm;
    private string asmLoc = "WPCalc";
    // Keep track of ALL methods
    private List<MethodInfo> methodList;
    // Keep track of ALL check boxes.
    private List<CheckBox> checkList;
    // Keep track of calculator groupings.
    private List<CalculatorInfo> calcGroups;

    protected void Page_Init(object sender, EventArgs e)
    {
        // Load the Assembly on load.
        try
        {
            asm = Assembly.Load(asmLoc);
            methodList = new List<MethodInfo>();
            checkList = new List<CheckBox>();
            calcGroups = new List<CalculatorInfo>();
            DisplayCalcOptions();
        }
        catch (Exception ex)
        {
            errorBox.Text = "ERROR: Failed to load assembly: " + asmLoc;
            errorBox.Text += ex.Message;
        }
    }

    // On page load draw everything the user selected in the menu.
    protected void Page_Load(object sender, EventArgs e)
    {
        // Make sure we group operations correctly.
        foreach (CalculatorInfo item in calcGroups)
        {
            int checkCount = 0;
            foreach (CheckBox check in item.Checkboxes)
                if (check.Checked)
                    checkCount++;

            // Create a control panel for all the operations
            item.ControlPanel.Controls.Clear();
            // Create a panel for the input fields
            item.InputPanel.Controls.Clear();

            try
            {
                // If checkcount > 0, add input boxes.
                if (checkCount > 0)
                    CreateInputBoxes(item.InputPanel, item.Kind);

                // Build all the buttons for each selected checkbox.
                foreach (CheckBox check in item.Checkboxes)
                {
                    if (check.Checked)
                    {
                        // Try to get the index of the operation stored in methodList.
                        int idx = 0;
                        int.TryParse(check.InputAttributes["value"], out idx);
                        MethodInfo m = methodList[idx];
                        Button but = new Button();
                        
                        // Save the opt as the value.
                        but.Attributes.Add("value", idx.ToString());
                        but.Text = OperatorToken(m.Name);
                        
                        // Set some internal properties to help us recover what op it as
                        if (item.Kind == CalcKind.SimpleCalc)
                            but.Attributes.Add("kind", "simple");
                        else if (item.Kind == CalcKind.ComplexCalc)
                            but.Attributes.Add("kind", "complex");
                        
                        but.Click += new EventHandler(OnCalculate);
                        item.ControlPanel.Controls.Add(but);
                    }
                }
            }
            catch (Exception exc)
            {
                errorBox.Text = "ERROR: " + exc.Message;
            }
        }
    }

    protected void DisplayCalcOptions()
    {
        // Make header cell.
        TableHeaderCell hdr = new TableHeaderCell();
        hdr.Text = "Calculator Studio";
        hdr.ColumnSpan = 2;

        // Make header row.
        TableHeaderRow hdrRow = new TableHeaderRow();
        hdrRow.Controls.Add(hdr);

        // Add to form.
        optTable.Controls.Add(hdrRow);
        calcPick.Controls.Add(optTable);

        // Read through the dll and get the interfaces.
        var simpleInterface = typeof(iSimpleCalc);
        var complexInterface = typeof(iComplexCalc);
        // Get only the types which implement the two interfaces.
        var simpleTypes = asm.GetTypes().Where(t => simpleInterface.IsAssignableFrom(t) && !t.IsInterface);
        var complexTypes = asm.GetTypes().Where(t => complexInterface.IsAssignableFrom(t) && !t.IsInterface);
        var allCalcTypes = simpleTypes.Union(complexTypes);

        int index = 0;
        foreach (var type in allCalcTypes)
        {
            TableHeaderRow typeRow = new TableHeaderRow();
            TableHeaderCell typeHdr = new TableHeaderCell();
            typeHdr.Text = "Calculator: " + type;
            typeHdr.ColumnSpan = 2;
            typeRow.Controls.Add(typeHdr);
            optTable.Controls.Add(typeRow);

            TableHeaderCell methodCol = new TableHeaderCell();
            methodCol.Text = "Methods";
            TableHeaderCell pickCol = new TableHeaderCell();
            pickCol.Text = "Pick";

            TableHeaderRow subHdrRow = new TableHeaderRow();
            subHdrRow.Controls.Add(methodCol);
            subHdrRow.Controls.Add(pickCol);

            optTable.Controls.Add(subHdrRow);

            // Add the appropriate panels for calculation.
            Panel controlPanel = new Panel();
            Panel inputPanel = new Panel();
            TableRow controlRow = new TableRow();
            TableCell controlCell = new TableCell();
            controlRow.Controls.Add(controlCell);
            controlCell.Controls.Add(controlPanel);

            TableRow inputRow = new TableRow();
            TableCell inputCell = new TableCell();
            inputRow.Controls.Add(inputCell);
            inputCell.Controls.Add(inputPanel);

            studioTable.Controls.Add(controlRow);
            studioTable.Controls.Add(inputRow);

            // Create a group.
            CalculatorInfo info = new CalculatorInfo();
            info.ControlPanel = controlPanel;
            info.InputPanel = inputPanel;
            if (simpleInterface.IsAssignableFrom(type))
                info.Kind = CalcKind.SimpleCalc;
            if (complexInterface.IsAssignableFrom(type))
                info.Kind = CalcKind.ComplexCalc;

            // Removes object inherited functions.
            // Source: http://stackoverflow.com/questions/12475417/dont-return-tostring-equals-gethashcode-gettype-with-type-getmethods
            var methods = type.GetMethods()
                                .Where(m => m.DeclaringType != typeof(object));

            foreach (var method in methods)
            {
                // Create a row.
                TableRow optRow = new TableRow();
                // Create a method and a checkbox for each one.
                TableCell methodName = new TableCell();
                methodName.Text = method.ToString();

                TableCell check = new TableCell();
                CheckBox cbox = new CheckBox();
                cbox.ID = "checkbox" + index;
                cbox.InputAttributes.Add("value", index.ToString());
                check.Controls.Add(cbox);

                optRow.Controls.Add(methodName);
                optRow.Controls.Add(check);
                optTable.Controls.Add(optRow);

                // Add the method to the method list.
                methodList.Add(method);
                checkList.Add(cbox);

                // Add to info group.
                info.Methods.Add(method);
                info.Checkboxes.Add(cbox);
                index++;
            }

            // Add group to list.
            calcGroups.Add(info);
        }

    }

    protected void DoSimpleMath(MethodInfo method)
    {
        float a1 = 0;
        float a2 = 0;

        var simple1 = calcPick.FindControl("simple1");
        var simple2 = calcPick.FindControl("simple2");

        if (simple1 != null && simple2 != null)
        {
            if (simple1 is TextBox && simple2 is TextBox)
            {
                TextBox t1 = (TextBox)simple1;
                TextBox t2 = (TextBox)simple2;

                if (float.TryParse(t1.Text, out a1) && float.TryParse(t2.Text, out a2))
                {
                    // Get the class that declared this type.
                    Type calc = method.DeclaringType;
                    // Invoke
                    Object obj = Activator.CreateInstance(calc);
                    var result = method.Invoke(obj, new object[] { a1, a2 });
                    // Output result
                    answerBox.Text = "RESULT: " + result.ToString();
                    // Clear the error box.
                    errorBox.Text = "";
                }
                else
                    errorBox.Text = "ERROR: Invalid input. Try again.";
            }
        }
        else
            errorBox.Text = "ERROR: Corrupt simple input boxes.";
    }

    protected void DoComplexMath(MethodInfo method)
    {
        float real1 = 0;
        float img1 = 0;
        float real2 = 0;
        float img2 = 0;

        var r1 = Form.FindControl("complexReal1");
        var i1 = Form.FindControl("complexImg1");
        
        var r2 = Form.FindControl("complexReal2");
        var i2 = Form.FindControl("complexImg2");

        if (r1 != null && i1 != null && r2 != null && i2 != null)
        {
            if (r1 is TextBox && i1 is TextBox && r2 is TextBox && i2 is TextBox)
            {
                TextBox r1Box = (TextBox)r1;
                TextBox i1Box = (TextBox)i1;
                TextBox r2Box = (TextBox)r2;
                TextBox i2Box = (TextBox)i2;

                if (float.TryParse(r1Box.Text, out real1) &&
                    float.TryParse(i1Box.Text, out img1) &&
                    float.TryParse(r2Box.Text, out real2) &&
                    float.TryParse(i2Box.Text, out img2))
                {
                    // Get the class that declared this type.
                    Type calc = method.DeclaringType;
                    // Invoke
                    Object obj = Activator.CreateInstance(calc);
                    var result = (cFloat)method.Invoke(obj, new object[] { new cFloat(real1, img1), new cFloat(real2, img2) });
                    // Output result
                    answerBox.Text = "RESULT: " + result.getReal() + " + " + result.getImg() + "i";
                    // Clear the error box.
                    errorBox.Text = "";
                }
                else
                    errorBox.Text = "ERROR: Invalid input. Try again.";
            }
        }
        else
            errorBox.Text = "ERROR: Corrupt complex input boxes.";
    }

    protected void DoMath(MethodInfo method)
    {
        try
        {
            // Simple calculator returns floats
            if (method.ReturnType == typeof(float))
            {
                DoSimpleMath(method);
            }
            
            // Complex calculator returns cFloats
            if (method.ReturnType == typeof(cFloat))
            {
                DoComplexMath(method);
            }
        }
        catch (Exception exc) { errorBox.Text = "ERROR: " + exc.Message; }
    }

    protected void OnCalculate(object sender, EventArgs args)
    {
        if (sender is Button)
        {
            Button b = (Button)sender;
            int opt = 0;

            string value = "";
            string kind = "";
            try
            {
                value = b.Attributes["value"];
                if (!int.TryParse(value, out opt))
                    errorBox.Text = "ERROR: Invalid operation.";

                kind = b.Attributes["kind"];

                MethodInfo method = methodList[opt];
                DoMath(method);
            }
            catch (Exception exc) { errorBox.Text = "ERROR: " + exc.Message;  }
        }
    }

    private void CreateInputBoxes(Panel inputPanel, CalcKind kind)
    {
        if (kind == CalcKind.SimpleCalc)
        {
            Label l1 = new Label();
            l1.Text = "val1";
            TextBox t1 = new TextBox();
            t1.ID = "simple1";

            Label l2 = new Label();
            l2.Text = "val2";
            TextBox t2 = new TextBox();
            t2.ID = "simple2";

            inputPanel.Controls.Add(l1);
            inputPanel.Controls.Add(t1);
            inputPanel.Controls.Add(l2);
            inputPanel.Controls.Add(t2);
            inputPanel.Controls.Add(new LiteralControl("<br />"));
            inputPanel.Controls.Add(new LiteralControl("<hr />"));
        }
        else if (kind == CalcKind.ComplexCalc)
        {
            Label l1 = new Label();
            l1.Text = "real1";
            TextBox t1 = new TextBox();
            t1.ID = "complexReal1";

            Label l2 = new Label();
            l2.Text = "img1";
            TextBox t2 = new TextBox();
            t2.ID = "complexImg1";

            Label l3 = new Label();
            l3.Text = "real2";
            TextBox t3 = new TextBox();
            t3.ID = "complexReal2";

            Label l4 = new Label();
            l4.Text = "img2";
            TextBox t4 = new TextBox();
            t4.ID = "complexImg2";

            inputPanel.Controls.Add(l1);
            inputPanel.Controls.Add(t1);
            inputPanel.Controls.Add(l2);
            inputPanel.Controls.Add(t2);
            inputPanel.Controls.Add(new LiteralControl("<br />"));
            inputPanel.Controls.Add(new LiteralControl("<br />"));
            inputPanel.Controls.Add(l3);
            inputPanel.Controls.Add(t3);
            inputPanel.Controls.Add(l4);
            inputPanel.Controls.Add(t4);
            inputPanel.Controls.Add(new LiteralControl("<br />"));
            inputPanel.Controls.Add(new LiteralControl("<hr />"));
        }
    }

    private string OperatorToken(string functionName)
    {
        if (functionName == "add")
            return "+";
        if (functionName == "subtract")
            return "-";
        if (functionName == "multiply")
            return "*";
        if (functionName == "divide")
            return "/";

        return "";
    }
    
</script>
