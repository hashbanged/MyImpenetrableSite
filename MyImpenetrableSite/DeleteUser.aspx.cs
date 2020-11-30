using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.Configuration;
using System.Data.SqlClient;

namespace MyImpenetrableSite
{
    public partial class DeleteUser : System.Web.UI.Page
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

            // Create a SqlConnection object
            SqlConnection conn = new SqlConnection(WebConfigurationManager.ConnectionStrings["MISConnectionString"].ToString());

            // DELETE statement and SqlCommand object
            // Actually, we don't delete the user. We just deactivate the user.

            int userId = int.Parse(Request.QueryString["Id"].ToString());

            SqlCommand cmdDelete = new SqlCommand(null, conn);

            cmdDelete.CommandText = "UPDATE Users " +
                "SET StatusId = @statusId " +
                "WHERE ID = @userId";

            SqlParameter paramUserId = new SqlParameter("@userId", System.Data.SqlDbType.Int);
            paramUserId.Value = userId;
            cmdDelete.Parameters.Add(paramUserId);

            SqlParameter paramStatusId = new SqlParameter("@statusId", System.Data.SqlDbType.Int);
            paramStatusId.Value = 2; // '2' signifies soft-deleted/"inactive" status
            cmdDelete.Parameters.Add(paramStatusId);

            conn.Open();
            cmdDelete.ExecuteNonQuery();
            conn.Close();

            Response.Redirect("Admin.aspx");
        }
    }
}