using Microsoft.AspNetCore.Mvc;
using MySql.Data.MySqlClient;
using BackendApp.Models;
using Microsoft.Extensions.Configuration;
using System.Collections.Generic;

namespace BackendApp.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class UsuariosController : ControllerBase
    {
        private readonly IConfiguration _config;
        private string cs;

        public UsuariosController(IConfiguration config)
        {
            _config = config;
        }

        [HttpGet]
        public IActionResult GetUsuarios()
        {
            var lista = new List<Usuario>();
            string cs = _config.GetConnectionString("DefaultConnection");

            using (var connection = new MySqlConnection(cs))
            {
                connection.Open();
                var command = new MySqlCommand("SELECT Id, Nombre, Email, Edad FROM Usuarios", connection);
                using (var reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        lista.Add(new Usuario
                        {
                            Id = reader.GetInt32("Id"),
                            Nombre = reader.GetString("Nombre"),
                            Email = reader.GetString("Email"),
                            Edad = reader.GetInt32("Edad")
                        });
                    }
                }
            }

            return Ok(lista); // ✅ variable correcta
        }

        [HttpPost]
        public IActionResult AddUsuario([FromBody] Usuario nuevoUsuario)
        {
            string cs = _config.GetConnectionString("DefaultConnection");

            using (var connection = new MySqlConnection(cs))
            {
                connection.Open();
                var command = new MySqlCommand(
                    "INSERT INTO Usuarios (Nombre, Email, Edad) VALUES (@n, @e, @ed)", connection);
                command.Parameters.AddWithValue("@n", nuevoUsuario.Nombre);
                command.Parameters.AddWithValue("@e", nuevoUsuario.Email);
                command.Parameters.AddWithValue("@ed", nuevoUsuario.Edad);
                command.ExecuteNonQuery();
            }

            return Ok(nuevoUsuario);
        }
        [HttpDelete("{id}")]
        public IActionResult DeleteUsuario(int id)
        {
            string cs = _config.GetConnectionString("DefaultConnection"); // ✅ conexión igual que en los otros métodos

            using (var connection = new MySqlConnection(cs))
            {
                connection.Open();
                var query = "DELETE FROM Usuarios WHERE Id = @Id";
                using (var command = new MySqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@Id", id);
                    var filasAfectadas = command.ExecuteNonQuery();

                    if (filasAfectadas == 0)
                        return NotFound(); // ❌ no encontró el registro
                }
            }


            return NoContent();
        }
        [HttpPut("{id}")]
        public IActionResult UpdateUsuario(int id, [FromBody] Usuario usuarioActualizado)
        {
            string cs = _config.GetConnectionString("DefaultConnection"); // ✅ conexión igual que en los otros métodos

            using (var connection = new MySqlConnection(cs))
            {
                connection.Open();
                var query = "UPDATE Usuarios SET Nombre = @n, Email = @e, Edad = @ed WHERE Id = @Id";
                using (var command = new MySqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@n", usuarioActualizado.Nombre);
                    command.Parameters.AddWithValue("@e", usuarioActualizado.Email);
                    command.Parameters.AddWithValue("@ed", usuarioActualizado.Edad);
                    command.Parameters.AddWithValue("@Id", id);
                    var filasAfectadas = command.ExecuteNonQuery();

                    if (filasAfectadas == 0)
                        return NotFound(); // ❌ no encontró el registro
                }
            }

            return Ok(usuarioActualizado);
        }



    }
    
    
}
