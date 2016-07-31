<%@ Page Title="DB Management" MasterPageFile="~/Site.Master" Language="C#" AutoEventWireup="true" CodeBehind="DBManagement.aspx.cs" Inherits="BMWeb2.DBManagement" %>

<asp:Content runat="server" ContentPlaceHolderID="MainContent">
    <link href="Styles.css" rel="stylesheet" />
    <script>
        function selectAllClients(){
            $(".client").prop("checked", $("#cboxSelectAllClients").prop("checked"));
        }
    </script>

    <h3>Database Management</h3>

    <p>This page copies the code from the DEV database onto the selected client databases. The order of creation/alteration of code is the following:</p>
    <ol>
        <li>Views</li>
        <li>Scalar-valued function</li>
        <li>Inline table-valued functions</li>
        <li>Multi-statement table-valued function</li>
    </ol>
    <p>You can also add new client's info in this page.</p>

    <h3>Client List</h3>

    <asp:Repeater ID="rpClients" runat="server">
        <HeaderTemplate>
            <table class="simpleTable">
                <tr>
                    <th><asp:CheckBox ID="cboxSelectAllClients" runat="server" OnCheckedChanged="cboxSelectAllClients_CheckedChanged" AutoPostBack="true"/></th>
                    <th>Client Name</th>
                    <th>RL User</th>
                    <th>RL Pass</th>
                    <th>Azure User</th>
                    <th>Azure Pass</th>
                    <th style="width: 250px;">Azure Server</th>
                    <th style="width: 120px;">Azure Database</th>
                </tr>
        </HeaderTemplate>
        <ItemTemplate>
            <tr>
                <td>
                    <asp:CheckBox ID="cboxSelect" runat="server" CssClass="client"/>
                    <%--<input id="cboxSelect" type="checkbox" runat="server" class="client" />--%>
                </td>
                <td><asp:Label id="lblName" runat="server" Text='<%# Eval("ClientName") %>'></asp:Label></td>
                <td><asp:Label id="lblRLUser" runat="server" Text='<%# Eval("RLUserID") %>'></asp:Label></td>
                <td><asp:Label id="lblRLPass" runat="server" Text='<%# Eval("RLPassword") %>'></asp:Label></td>
                <td><asp:Label id="lblAzureUser" runat="server" Text='<%# Eval("AzureUserID") %>'></asp:Label></td>
                <td><asp:Label id="lblAzurePass" runat="server" Text='<%# Eval("AzurePassword") %>'></asp:Label></td>
                <td><asp:Label id="lblAzureServer" runat="server" Text='<%# Eval("AzureServer") %>'></asp:Label></td>
                <td><asp:Label id="lblAzureDB" runat="server" Text='<%# Eval("AzureDBName") %>'></asp:Label></td>
            </tr>
        </ItemTemplate>
        <FooterTemplate>
            <tr>
                <td></td>
                <td><asp:TextBox id="tboxName" runat="server" Width="100px"></asp:TextBox></td>
                <td><asp:TextBox ID="tboxRLUser" runat="server" Width="100px"></asp:TextBox></td>
                <td><asp:TextBox ID="tboxRLPass" runat="server" Width="100px"></asp:TextBox></td>
                <td><asp:TextBox ID="tboxAzureUser" runat="server" Width="100px"></asp:TextBox></td>
                <td><asp:TextBox ID="tboxAzurePass" runat="server" Width="100px"></asp:TextBox></td>
                <td><asp:TextBox ID="tboxAzureServer" runat="server" Width="247px"></asp:TextBox></td>
                <td><asp:TextBox ID="tboxAzureDB" runat="server" Width="117px"></asp:TextBox></td>
            </tr>
            <tr>
                <td style="border: 0px;"></td>
                <td style="border: 0px;"><asp:Button ID="btnAddClient" runat="server" Text="Add Client" OnClick="btnAddClient_Click"/></td>
            </tr>
            </table>
        </FooterTemplate>
    </asp:Repeater>

    <asp:Button ID="btnApplyStructure" runat="server" Text="Apply structure to selected" OnClick="btnApplyStructure_Click" />

    <br />
    <asp:Label ID="lblMessage" runat="server" CssClass="alert"></asp:Label>

    <br /><br />
    <asp:Label ID="lblMessage2" runat="server"></asp:Label>

</asp:Content>
