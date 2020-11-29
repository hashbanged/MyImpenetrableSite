using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Data.SqlClient;
using System.Web.Configuration;

namespace MyImpenetrableSite
{
    public partial class Login : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {

        }

        protected void btnLogin_Click(object sender, EventArgs e)
        {
            // Create a SQL connection object
            SqlConnection conn = new SqlConnection(WebConfigurationManager.ConnectionStrings["MISConnectionString"].ToString());

            string username = txtUsername.Text.Trim().ToLower();
            string password = txtPassword.Text.Trim();

            SqlCommand cmd = new SqlCommand(null, conn);

            cmd.CommandText = "SELECT * FROM Users " +
                "WHERE Username = @username " +
                "AND Password = @password";

            SqlParameter paramUsername = new SqlParameter("@username", System.Data.SqlDbType.NVarChar, 50);
            paramUsername.Value = username;
            cmd.Parameters.Add(paramUsername);

            SqlParameter paramPassword = new SqlParameter("@password", System.Data.SqlDbType.NVarChar, 1000);
            // To-do: Use SHA256 hash function to compare database-stored value.
            paramPassword.Value = password;
            cmd.Parameters.Add(paramPassword);

            conn.Open();
            cmd.Prepare();
            SqlDataReader reader = cmd.ExecuteReader();

            if (!reader.HasRows)
            {
                lblLoginError.Text = "That didn't work. Please try again.";
            }
            else
            {
                reader.Read();

                int userId = int.Parse(reader["Id"].ToString());
                int roleId = int.Parse(reader["RoleId"].ToString());
                int statusId = int.Parse(reader["StatusId"].ToString());

                reader.Close();
                conn.Close();

                // Set the session variables to be used in a security context. 
                Session["user_role_id"] = roleId;
                Session["user_user_id"] = userId;
                Session["user_status_id"] = statusId;

                // To-do: Set userId, statusId, and roleId in new session.

                if (roleId == 1)  // Administrator
                {
                    Response.Redirect("Admin.aspx");
                }
                else
                {
                    if (statusId == 2)
                    {
                        lblLoginError.Text = "Your account is inactive. Please contact the administrator to deactivate your account first.";
                    }
                    else
                    {
                        Response.Redirect("Members.aspx?Id=" + userId);
                    }

                }
            }
        }
    }
}