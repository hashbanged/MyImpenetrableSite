using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.Configuration;
using System.Data.SqlClient;
using System.Data;

namespace MyImpenetrableSite
{
    public partial class Members : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!Page.IsPostBack)
            {
                // Redirect to the login page if no session user is established.
                if (null == Session["user_user_id"])
                {
                    Response.Redirect("Login.aspx");
                }

                // Get the user id from the query string
                int userId = int.Parse(Request.QueryString["Id"].ToString());
                int sessionUserId = int.Parse(Session["user_user_id"].ToString());
                int sessionRoleId = int.Parse(Session["user_role_id"].ToString());

                // Redirect non-admin user ID mismatches to the session user's member page.
                if (sessionRoleId != 1 && (userId != sessionUserId))
                {
                    Response.Redirect("Members.aspx?Id=" + sessionUserId);
                }


                // SqlConnection object
                SqlConnection conn = new SqlConnection(WebConfigurationManager.ConnectionStrings["MISConnectionString"].ToString());

                SqlCommand cmdSelect = new SqlCommand(null, conn);

                cmdSelect.CommandText = "SELECT * FROM Users WHERE ID = @userId";

                SqlParameter paramUserId = new SqlParameter("userId", SqlDbType.Int);
                paramUserId.Value = userId;
                cmdSelect.Parameters.Add(paramUserId);

                conn.Open();
                cmdSelect.Prepare();
                SqlDataReader reader = cmdSelect.ExecuteReader();

                if (reader.HasRows)
                {
                    reader.Read();
                    lblUsername.Text = reader["Username"].ToString();
                    txtFirstName.Text = reader["FirstName"].ToString();
                    txtLastName.Text = reader["LastName"].ToString();
                    txtEmail.Text = reader["Email"].ToString();
                    txtPhone.Text = reader["Phone"].ToString();
                }
                reader.Close();
                conn.Close();
            }
        }

        protected void btnUpdateProfile_Click(object sender, EventArgs e)
        {
            int userId = int.Parse(Request.QueryString["Id"].ToString());
            string firstName = txtFirstName.Text.Trim();
            string lastName = txtLastName.Text.Trim();
            string email = txtEmail.Text.Trim();
            string phone = txtPhone.Text.Trim();

            // SqlConnection
            SqlConnection conn = new SqlConnection(WebConfigurationManager.ConnectionStrings["MISConnectionString"].ToString());

            // SqlCommand
            SqlCommand cmdUpdate = new SqlCommand(null, conn);

            // UPDATE statement
            cmdUpdate.CommandText = "UPDATE Users " +
                "SET FirstName = @firstName, LastName = @lastName, Email = @email, Phone = @phone " +
                "WHERE ID = @userId";

            SqlParameter paramFirstName = new SqlParameter("@firstName", SqlDbType.NVarChar, 100);
            paramFirstName.Value = firstName;
            cmdUpdate.Parameters.Add(paramFirstName);

            SqlParameter paramLastName = new SqlParameter("@lastName", SqlDbType.NVarChar, 100);
            paramLastName.Value = lastName;
            cmdUpdate.Parameters.Add(paramLastName);

            SqlParameter paramEmail = new SqlParameter("@email", SqlDbType.NVarChar, 250);
            paramEmail.Value = txtEmail.Text.Trim();
            cmdUpdate.Parameters.Add(paramEmail);

            SqlParameter paramPhone = new SqlParameter("@phone", SqlDbType.NVarChar, 50);
            paramPhone.Value = txtPhone.Text.Trim();
            cmdUpdate.Parameters.Add(paramPhone);

            SqlParameter paramUserId = new SqlParameter("@userId", SqlDbType.Int);
            paramUserId.Value = userId;
            cmdUpdate.Parameters.Add(paramUserId);

            conn.Open();
            cmdUpdate.Prepare();
            cmdUpdate.ExecuteNonQuery();
            conn.Close();
        }
    }
}