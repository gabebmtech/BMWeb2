using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace BMWeb2
{
    public partial class DBManagement : System.Web.UI.Page
    {

        protected string generateConnectionString(string server, string database, string userID, string pass)
        {
            return "Server=tcp:" + server + ";Database=" + database + ";User ID=" + userID + ";Password=" + pass + ";Trusted_Connection=False;Encrypt=True;Connection Timeout=7200; ";
        }

        protected SqlDataReader ExecuteReader(SqlCommand comm, string connectionString)
        {
            SqlConnection conn = new SqlConnection(connectionString);
            comm.Connection = conn;
            conn.Open();

            return comm.ExecuteReader(CommandBehavior.CloseConnection);
        }
        public Object ExecuteScalar(SqlCommand cmd, string connectionString)
        {
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                cmd.Connection = conn;
                conn.Open();
                return cmd.ExecuteScalar();
            }
        }
        public void ExecuteNonQuery(SqlCommand cmd, string connectionString)
        {
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                cmd.Connection = conn;
                conn.Open();

                cmd.ExecuteNonQuery();
            }
        }

        // ****************** ACTIONS ****************************
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                string connstr_bmtech = generateConnectionString("bmtech.database.windows.net", "bmtech", "bmdev", "BMTech2016");

                SqlCommand comm = new SqlCommand("System_Clients_GetAllInfo");
                comm.CommandType = CommandType.StoredProcedure;

                DataTable tbl = new DataTable();
                tbl.Load(ExecuteReader(comm, connstr_bmtech));

                rpClients.DataSource = tbl;
                rpClients.DataBind();
            }
        }
        protected void btnApplyStructure_Click(object sender, EventArgs e)
        {
            //Get code to run against DEV database
            string connstr_bmtech = generateConnectionString("bmtech.database.windows.net", "bmtech", "bmdev", "BMTech2016");

            SqlCommand comm1 = new SqlCommand("System_Admin_GetDevCode");
            comm1.CommandType = CommandType.StoredProcedure;

            string command1 = (string)ExecuteScalar(comm1, connstr_bmtech);

            lblMessage.Text = command1;


            //Run code against DEV database
            string connstr_dev = generateConnectionString("tzumi.database.windows.net", "tzumi", "tzumi", "ProGlass2016");

            SqlCommand comm2 = new SqlCommand(command1);

            DataTable structure = new DataTable();
            structure.Load(ExecuteReader(comm2, connstr_dev));

            string message2 = "PROCESSED:<br/ ><br />";
            
            //Generate given structure in the target databases
            foreach (RepeaterItem row in rpClients.Items)
            {
                try
                {
                    if (row.ItemType == ListItemType.Item || row.ItemType == ListItemType.AlternatingItem)
                    {
                        //Execute script only for the checked clients
                        if (((CheckBox)row.FindControl("cboxSelect2")).Checked)
                        {
                            //Generate connection string
                            string server = ((Label) row.FindControl("lblAzureServer")).Text;
                            string database = ((Label)row.FindControl("lblAzureDB")).Text;
                            string user = ((Label)row.FindControl("lblAzureUser")).Text;
                            string pass = ((Label)row.FindControl("lblAzurePass")).Text;

                            string connstr_target = generateConnectionString(server, database, user, pass);

                            message2 += "<br/><br/><br/>@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@" +
                                        "<br/>" + connstr_target + "<br/>" +
                                        "@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@<br/><br/><br/>";

                            foreach (DataRow srow in structure.Rows)
                            {
                                //Generate command
                                    message2 += "~~~~~~~~~~~~~~~~~~~~~~~~~~~~~<br />~~~~~~~~~~~~~~~~~~~~~~~~~~~~~ <br />";
                                string name = srow["NAME"].ToString();
                                    message2 += "NAME: " + name + "<br />";
                                string type = srow["TYPE"].ToString().ToUpper();
                                    message2 += "TYPE: " + type + "<br /> ";
                                string definition = srow["DEFINITION"].ToString().Replace("CREATE", "ALTER");
                                    //message2 += "DEFINITION<br/>" + definition.Replace("\n", "<br />").Replace("\t", "....") + " <br />"; 

                                string command = "";

                                if (type == "V ")
                                {
                                    command = //"USE " + database + "; " +
                                        "IF NOT EXISTS (SELECT * FROM sys.objects WHERE [TYPE] = 'V' AND OBJECT_NAME(OBJECT_ID) = '" + name + "')\n" +
                                        "   EXEC('CREATE VIEW " + name + " AS SELECT 0 AS [Name];')";
                                }
                                if (type == "P ")
                                {
                                    command = //"USE " + database + "; " +
                                        "IF NOT EXISTS (SELECT * FROM sys.objects WHERE [TYPE] = 'P' AND OBJECT_NAME(OBJECT_ID) = '" + name + "')\n" +
                                        "   EXEC('CREATE PROCEDURE " + name + " AS BEGIN SET NOCOUNT ON; END')";
                                }
                                if (type == "IF")
                                {
                                    command = //"USE " + database + "; " +
                                        "IF NOT EXISTS (SELECT * FROM sys.objects WHERE [TYPE] = 'IF' AND OBJECT_NAME(OBJECT_ID) = '" + name + "')\n" +
                                        "   EXEC('CREATE FUNCTION " + name + "() RETURNS TABLE AS RETURN ( SELECT 0 AS Val )')";
                                }
                                if (type == "TF")
                                {
                                    command = //"USE " + database + "; " +
                                        "IF NOT EXISTS (SELECT * FROM sys.objects WHERE [TYPE] = 'TF' AND OBJECT_NAME(OBJECT_ID) = '" + name + "')\n" +
                                        "   EXEC('CREATE FUNCTION " + name + "() RETURNS @tbl TABLE (Val INT) AS BEGIN RETURN; END;')";
                                }
                                if (type == "FN")
                                {
                                    command = //"USE " + database + "; " +
                                        "IF NOT EXISTS (SELECT * FROM sys.objects WHERE [TYPE] = 'FN' AND OBJECT_NAME(OBJECT_ID) = '" + name + "')\n" +
                                        "   EXEC('CREATE FUNCTION " + name + "() RETURNS INT AS BEGIN RETURN 0; END;')";
                                }

                                    //message2 += "COMMAND:<br/>" + command + "<br />";
                                //Create if it doesn't exist
                                SqlCommand comm = new SqlCommand(command);
                                ExecuteNonQuery(comm, connstr_target);

                                    message2 += "--> command executed<br/>";

                                //Alter existing code
                                SqlCommand def = new SqlCommand(definition);
                                ExecuteNonQuery(def, connstr_target);

                                    message2 += "--> definition executed<br/><br/>";
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    lblMessage.Text = "Error: " + ex.Message;
                }

                lblMessage2.Text = message2;
            }

            lblMessage.Text = "Deployment COMPLETE";
        }

        protected void btnAddClient_Click(object sender, EventArgs e)
        {
            string clientName = ((TextBox)rpClients.Controls[rpClients.Controls.Count - 1].Controls[0].FindControl("tboxName")).Text;
            string rlUserID = ((TextBox)rpClients.Controls[rpClients.Controls.Count - 1].Controls[0].FindControl("tboxRLUser")).Text;
            string rlPass = ((TextBox)rpClients.Controls[rpClients.Controls.Count - 1].Controls[0].FindControl("tboxRLPass")).Text;
            string azureUser = ((TextBox)rpClients.Controls[rpClients.Controls.Count - 1].Controls[0].FindControl("tboxAzureUser")).Text;
            string azurePass = ((TextBox)rpClients.Controls[rpClients.Controls.Count - 1].Controls[0].FindControl("tboxAzurePass")).Text;
            string azureServer = ((TextBox)rpClients.Controls[rpClients.Controls.Count - 1].Controls[0].FindControl("tboxAzureServer")).Text;
            string azureDB = ((TextBox)rpClients.Controls[rpClients.Controls.Count - 1].Controls[0].FindControl("tboxAzureDB")).Text;

            if (clientName != "" && rlUserID != "" && rlPass != "" && azureUser != "" && azurePass != "" && azureServer != "" && azureDB != "")
            {
                string connstr_bmtech = generateConnectionString("bmtech.database.windows.net", "bmtech", "bmdev", "BMTech2016");

                SqlCommand comm = new SqlCommand("System_Clients_Manage");
                comm.CommandType = CommandType.StoredProcedure;

                comm.Parameters.Add("@command", SqlDbType.NVarChar, 30).Value = "ADD";
                comm.Parameters.Add("@clientName", SqlDbType.NVarChar, 255).Value = clientName;
                comm.Parameters.Add("@rlUserID", SqlDbType.NVarChar, 255).Value = rlUserID;
                comm.Parameters.Add("@rlPass", SqlDbType.NVarChar, 255).Value = rlPass;
                comm.Parameters.Add("@azureUserID", SqlDbType.NVarChar, 255).Value = azureUser;
                comm.Parameters.Add("@azurePass", SqlDbType.NVarChar, 255).Value = azurePass;
                comm.Parameters.Add("@azureServer", SqlDbType.NVarChar, 255).Value = azureServer;
                comm.Parameters.Add("@azureDB", SqlDbType.NVarChar, 255).Value = azureDB;

                DataTable tbl = new DataTable();
                tbl.Load(ExecuteReader(comm, connstr_bmtech));

                rpClients.DataSource = tbl;
                rpClients.DataBind();
            }
        }

        protected void cboxSelectAllClients_CheckedChanged(object sender, EventArgs e)
        {
            foreach (RepeaterItem row in rpClients.Items)
            {
                if (row.ItemType == ListItemType.Item || row.ItemType == ListItemType.AlternatingItem)
                    ((CheckBox)row.FindControl("cboxSelect")).Checked = true;
            }
        }
    }
}