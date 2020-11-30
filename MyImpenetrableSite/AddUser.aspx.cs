using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Data.SqlClient;
using System.Web.Configuration;
using System.Data;

namespace MyImpenetrableSite
{
    public partial class AddUser : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            int sessionRoleId = int.Parse(Session["user_role_id"].ToString());
            int sessionStatusId = int.Parse(Session["user_status_id"].ToString());

            // Redirect to the login page if the session user doesn't exist or 
            // is invalid (is soft-deleted or doesn't have the admin role).
            if (1 != sessionStatusId || 1 != sessionRoleId)
            {
                Response.Redirect("Login.aspx");
            }
        }

        protected void btnAddUser_Click(object sender, EventArgs e)
        {
            // Create a SqlConnection object
            SqlConnection conn = new SqlConnection(WebConfigurationManager.ConnectionStrings["MISConnectionString"].ToString());

            string firstName = Request.Form["txtFirstName"].ToString().Trim();
            string lastName = txtLastName.Text.Trim();
            string currentDateTime = DateTime.Now.ToString();

            // Parameterize the SQL statement values.
            // See: https://docs.microsoft.com/en-us/dotnet/api/system.data.sqlclient.sqlcommand.prepare?view=dotnet-plat-ext-5.0

            SqlCommand cmdInsert = new SqlCommand(null, conn);

            cmdInsert.CommandText = "INSERT INTO Users " +
                "(FirstName, LastName, Email, Phone, Username, Password, RoleId, StatusId, LastLoginTime) " +
                "VALUES " +
                "(@firstName, @lastName, @email, @phone, @username, @password, @roleId, @statusId, @currentTimestamp)";

            SqlParameter paramFirstName = new SqlParameter("@firstName", SqlDbType.Text, 100);
            paramFirstName.Value = firstName;
            cmdInsert.Parameters.Add(paramFirstName);

            SqlParameter paramLastName = new SqlParameter("@lastName", SqlDbType.Text, 100);
            paramLastName.Value = lastName;
            cmdInsert.Parameters.Add(paramLastName);

            SqlParameter paramEmail = new SqlParameter("@email", SqlDbType.Text, 250);
            paramEmail.Value = txtEmail.Text.Trim();
            cmdInsert.Parameters.Add(paramEmail);

            SqlParameter paramPhone = new SqlParameter("@phone", SqlDbType.Text, 50);
            paramPhone.Value = txtPhone.Text.Trim();
            cmdInsert.Parameters.Add(paramPhone);

            SqlParameter paramUsername = new SqlParameter("@username", SqlDbType.Text, 50);
            paramUsername.Value = firstName[0].ToString().ToLower() + lastName.ToLower();
            cmdInsert.Parameters.Add(paramUsername);

            SqlParameter paramPassword = new SqlParameter("@password", SqlDbType.Text, 1000);
            // To-do: Use SHA256 hash replacement.
            paramPassword.Value = firstName + "." + lastName;
            cmdInsert.Parameters.Add(paramPassword);

            SqlParameter paramRoleId = new SqlParameter("@roleId", SqlDbType.Int);
            paramRoleId.Value = 2;
            cmdInsert.Parameters.Add(paramRoleId);

            SqlParameter paramStatusId = new SqlParameter("@statusId", SqlDbType.Int);
            paramStatusId.Value = 1;
            cmdInsert.Parameters.Add(paramStatusId);

            SqlParameter paramCurrentTimestamp = new SqlParameter("@currentTimestamp", SqlDbType.DateTime);
            paramCurrentTimestamp.Value = currentDateTime;
            cmdInsert.Parameters.Add(paramCurrentTimestamp);

            conn.Open();
            cmdInsert.Prepare();
            cmdInsert.ExecuteNonQuery();
            conn.Close();

            Response.Redirect("Admin.aspx");
        }
    }
}