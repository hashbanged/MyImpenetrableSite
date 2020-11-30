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
            string username = txtUsername.Text.Trim().ToLower();
            string passwordPlain = txtPassword.Text.Trim();

            // Create a SQL connection object
            SqlConnection conn = new SqlConnection(WebConfigurationManager.ConnectionStrings["MISConnectionString"].ToString());
            SqlCommand cmd = new SqlCommand(null, conn);

            cmd.CommandText = "SELECT * FROM Users " +
                "WHERE Username = @username ";

            SqlParameter paramUsername = new SqlParameter("@username", System.Data.SqlDbType.NVarChar, 50);
            paramUsername.Value = username;
            cmd.Parameters.Add(paramUsername);

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

                string passwordStored = reader["Password"].ToString();

                int userId = int.Parse(reader["Id"].ToString());
                int roleId = int.Parse(reader["RoleId"].ToString());
                int statusId = int.Parse(reader["StatusId"].ToString());

                reader.Close();
                conn.Close();

                // Perform two types of password checks: 
                //   1) a legacy check for plaintext passwords (to be deprecated and removed)
                //   2) a hash verification for users created after this commit.

                if (passwordPlain == passwordStored || Hasher.Verify(passwordPlain, passwordStored))
                {
                    // Set the session variables to be used in a security context. 
                    Session["user_role_id"] = roleId;
                    Session["user_user_id"] = userId;
                    Session["user_status_id"] = statusId;

                    if (roleId == 1)  // Administrator
                    {
                        Response.Redirect("Admin.aspx");
                    }
                    else
                    {
                        if (statusId == 2)
                        {
                            lblLoginError.Text = "Your account is inactive. Please contact the administrator to reactivate your account first.";
                        }
                        else
                        {
                            Response.Redirect("Members.aspx?Id=" + userId);
                        }
                    }
                }
                else
                {
                    lblLoginError.Text = "That didn't work. Please try again.";
                }
            }
        }
    }
}