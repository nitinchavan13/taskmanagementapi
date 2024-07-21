using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Web.Http.Cors;
using TaskManagerApi.db;
using TaskManagerApi.Models;

namespace TaskManagerApi.Controllers
{
    [EnableCors(origins: "*", headers: "*", methods: "*")]
    [RoutePrefix("api/v1/internal")]
    public class TaskController : BaseController
    {
        private DBHelper _dbHelper;
        public TaskController()
        {
            _dbHelper = new DBHelper();
            _dbHelper.CreateDBObjects(ConfigurationManager.AppSettings["mysqlConnectionString"], DBHelper.DbProviders.MySql);
        }

        [Authorize]
        [HttpGet]
        [Route("task/getAll")]
        public List<Task> Get()
        {
            List<Task> list = new List<Task>();
            _dbHelper.AddParameter("p_userId", this.GetUserId());
            var reader = _dbHelper.ExecuteReader("sp_get_tasks", System.Data.CommandType.StoredProcedure, System.Data.ConnectionState.Open);
            try
            {
                if (reader.HasRows)
                {
                    while (reader.Read())
                    {
                        list.Add(new Task()
                        {
                            Id = Convert.ToInt32(reader["id"]),
                            Title = reader["title"].ToString(),
                            DueDate = Convert.ToDateTime(reader["duedate"]),
                            Description = reader["description"].ToString(),
                            Status = reader["status"].ToString(),
                        });
                    }
                }
                return list;
            }
            catch (Exception ex)
            {

                throw new Exception(ex.Message);
            }
            finally
            {
                if (_dbHelper.connection.State != System.Data.ConnectionState.Closed)
                {
                    _dbHelper.connection.Close();
                    _dbHelper.connection.Dispose();
                }
            }
        }

        [Authorize]
        [HttpGet]
        [Route("task/getDetails/{id}")]
        public Task Get(int id)
        {
            Task task = new Task();
            _dbHelper.AddParameter("p_userId", this.GetUserId());
            _dbHelper.AddParameter("p_taskId", id);
            var reader = _dbHelper.ExecuteReader("sp_get_task_details", System.Data.CommandType.StoredProcedure, System.Data.ConnectionState.Open);
            try
            {
                if (reader.HasRows)
                {
                    while (reader.Read())
                    {
                        task = (new Task()
                        {
                            Id = Convert.ToInt32(reader["id"]),
                            Title = reader["title"].ToString(),
                            DueDate = Convert.ToDateTime(reader["duedate"]),
                            Description = reader["description"].ToString(),
                            Status = reader["status"].ToString(),
                        });
                    }
                }
                return task;
            }
            catch (Exception ex)
            {

                throw new Exception(ex.Message);
            }
            finally
            {
                if (_dbHelper.connection.State != System.Data.ConnectionState.Closed)
                {
                    _dbHelper.connection.Close();
                    _dbHelper.connection.Dispose();
                }
            }
        }

        [Authorize]
        [HttpPost]
        [Route("task/addTask")]
        public void Post(Task model)
        {
            _dbHelper.AddParameter("p_userId", this.GetUserId());
            _dbHelper.AddParameter("p_taskTitle", model.Title);
            _dbHelper.AddParameter("p_taskDescription", model.Description);
            _dbHelper.AddParameter("p_taskDueDate", model.DueDate);
            try
            {
                _dbHelper.ExecuteScaler("sp_add_task", System.Data.CommandType.StoredProcedure, System.Data.ConnectionState.Open);
            }
            catch (Exception ex)
            {

                throw new Exception(ex.Message);
            }
            finally
            {
                if (_dbHelper.connection.State != System.Data.ConnectionState.Closed)
                {
                    _dbHelper.connection.Close();
                    _dbHelper.connection.Dispose();
                }
            }
        }

        [Authorize]
        [HttpPut]
        [Route("task/updateTask")]
        public void Put(Task model)
        {
            _dbHelper.AddParameter("p_userId", this.GetUserId());
            _dbHelper.AddParameter("p_taskTitle", model.Title);
            _dbHelper.AddParameter("p_taskDescription", model.Description);
            _dbHelper.AddParameter("p_taskDueDate", model.DueDate);
            _dbHelper.AddParameter("p_taskId", model.Id);
            _dbHelper.AddParameter("p_taskStatus", model.Status);
            try
            {
                _dbHelper.ExecuteScaler("sp_update_task", System.Data.CommandType.StoredProcedure, System.Data.ConnectionState.Open);
            }
            catch (Exception ex)
            {

                throw new Exception(ex.Message);
            }
            finally
            {
                if (_dbHelper.connection.State != System.Data.ConnectionState.Closed)
                {
                    _dbHelper.connection.Close();
                    _dbHelper.connection.Dispose();
                }
            }
        }

        [Authorize]
        [HttpDelete]
        [Route("task/deleteTask/{id}")]
        public void Delete(int id)
        {
            _dbHelper.AddParameter("p_userId", this.GetUserId());
            _dbHelper.AddParameter("p_taskId", id);
            try
            {
                _dbHelper.ExecuteScaler("sp_delete_task", System.Data.CommandType.StoredProcedure, System.Data.ConnectionState.Open);
            }
            catch (Exception ex)
            {

                throw new Exception(ex.Message);
            }
            finally
            {
                if (_dbHelper.connection.State != System.Data.ConnectionState.Closed)
                {
                    _dbHelper.connection.Close();
                    _dbHelper.connection.Dispose();
                }
            }
        }
    }
}
