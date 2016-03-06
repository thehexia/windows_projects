using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.HtmlControls;
using System.Reflection;
using System.Drawing;

namespace CalcStudio
{
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


  public partial class Calc : System.Web.UI.Page
  {
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
        errorBox.Text = "*Failed to load assembly.";
        errorBox.Text += ex.Message;
      }
    }

    protected void Page_Load(object sender, EventArgs e)
    {
      foreach (CalculatorInfo item in calcGroups)
      {
        int checkCount = 0;
        foreach (CheckBox check in item.Checkboxes)
          if (check.Checked)
            checkCount++;

        item.ControlPanel.Controls.Clear();
        item.InputPanel.Controls.Clear();

        // If checkcount > 0, add input boxes.
        if (checkCount > 0)
          CreateInputBoxes(item.InputPanel, item.Kind);

        foreach (CheckBox check in item.Checkboxes)
        {
          if (check.Checked)
          {
            int idx = 0;
            int.TryParse(check.InputAttributes["value"], out idx);
            MethodInfo m = methodList[idx];
            Button but = new Button();
            // but.Attributes.Add("value", idx.ToString());
            but.Text = OperatorToken(m.Name);
            but.Click += new EventHandler(OnCalculate);
            item.ControlPanel.Controls.Add(but);
          }
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

    protected void OnCalculate(object sender, EventArgs args)
    {
      Button b = (Button)sender;
      errorBox.Text += "RESULT: " + b.Text;
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
        t4.ID = "complexImg1";

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
 
  } // end class

}