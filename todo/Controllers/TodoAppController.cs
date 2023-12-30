using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Data;
using System.Data.SqlClient;

namespace todo.Controllers
{
    [ApiController]
    [Route("/")]
    public class TodoAppController : ControllerBase
    {
        private IConfiguration _configuration;
        public TodoAppController(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        [HttpGet]
        [Route("GetTest")]
        public JsonResult GetTest()
        {
            return new JsonResult("test");
        }

        [HttpGet]
        [Route("GetTodos")]
        public JsonResult GetTodos()
        {
            string query = "select * from [dbo].[todos] ORDER BY id DESC";
            DataTable table = new DataTable();
            string sqlDatasource = _configuration.GetConnectionString("todoAppDBCon");
            SqlDataReader myReader;
            using(SqlConnection myCon=new SqlConnection(sqlDatasource))
            {
                myCon.Open();
                using(SqlCommand myCommand = new SqlCommand(query, myCon))
                {
                    myReader = myCommand.ExecuteReader();
                    table.Load(myReader);
                    myReader.Close();
                    myCon.Close();
                }
            }

            return new JsonResult(table);
        }

        public class TodoInputModel
        {
            public string Details { get; set; }
            public int Priority { get; set; }
            public bool Completed { get; set; }
        }

        [HttpPost]
        [Route("AddTodo")]
        public JsonResult AddTodo([FromBody] TodoInputModel inputModel)
        {
            string query = "insert into [dbo].[todos] values(@details, @priority, @completed)";
            DataTable table = new DataTable();
            string sqlDatasource = _configuration.GetConnectionString("todoAppDBCon");
            SqlDataReader myReader;
            using (SqlConnection myCon = new SqlConnection(sqlDatasource))
            {
                myCon.Open();
                using (SqlCommand myCommand = new SqlCommand(query, myCon))
                {
                    myCommand.Parameters.AddWithValue("details", inputModel.Details);
                    myCommand.Parameters.AddWithValue("priority", inputModel.Priority);
                    myCommand.Parameters.AddWithValue("completed", false);
                    myReader = myCommand.ExecuteReader();
                    table.Load(myReader);
                    myReader.Close();
                    myCon.Close();
                }
            }

            return new JsonResult("Added Successfully");
        }

        [HttpDelete]
        [Route("DeleteTodo")]
        public JsonResult DeleteTodo(int id)
        {
            string query = "delete from [dbo].[todos] where id=@id";
            DataTable table = new DataTable();
            string sqlDatasource = _configuration.GetConnectionString("todoAppDBCon");
            SqlDataReader myReader;
            using (SqlConnection myCon = new SqlConnection(sqlDatasource))
            {
                myCon.Open();
                using (SqlCommand myCommand = new SqlCommand(query, myCon))
                {
                    myCommand.Parameters.AddWithValue("@id", id);
                    myReader = myCommand.ExecuteReader();
                    table.Load(myReader);
                    myReader.Close();
                    myCon.Close();
                }
            }

            return new JsonResult("Deleted Successfully");
        }

        [HttpPut]
        [Route("UpdateTodo")]
        public JsonResult UpdateTodo(int id, [FromBody] TodoInputModel inputModel)
        {
            string query = "update [dbo].[todos] set details = @details, priority = @priority, completed = @completed where id = @id";
            DataTable table = new DataTable();
            string sqlDatasource = _configuration.GetConnectionString("todoAppDBCon");
            SqlDataReader myReader;
            using (SqlConnection myCon = new SqlConnection(sqlDatasource))
            {
                myCon.Open();
                using (SqlCommand myCommand = new SqlCommand(query, myCon))
                {
                    myCommand.Parameters.AddWithValue("details", inputModel.Details);
                    myCommand.Parameters.AddWithValue("priority", inputModel.Priority);
                    myCommand.Parameters.AddWithValue("completed", inputModel.Completed);
                    myCommand.Parameters.AddWithValue("@id", id);
                    myReader = myCommand.ExecuteReader();
                    table.Load(myReader);
                    myReader.Close();
                    myCon.Close();
                }
            }

            return new JsonResult("Updated Successfully");
        }
    }
}
