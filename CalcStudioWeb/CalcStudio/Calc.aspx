<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Calc.aspx.cs" Inherits="CalcStudio.Calc" %>

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
    <asp:Label ID="errorBox" RunAt="server" />
    <asp:Label ID="answerBox" runat="server"></asp:Label>
</body>
</html>
