using ReactTest.Models;
using System;
using System.Linq;
using System.Security.Cryptography;
using System.Web.Http;

namespace ReactTest.Controllers
{
    public class ValuesController : ApiController
    {
        // POST api/values/login
        [HttpPost]
        [Route("api/values/login")]
        public IHttpActionResult Login([FromBody] Utenti request)
        {
            TestDbContext db = new TestDbContext();
            var utente = db.Utenti.Where(u => u.Username == request.Username).FirstOrDefault();
            if (utente != null)
            {
                bool isPasswordValid = VerifyPassword(request.Psw, utente.Psw);
                if (isPasswordValid)
                {
                    // Genera un token di sessione e restituiscilo
                    string sessionToken = GenerateSessionToken();
                    return Ok(new { token = sessionToken });


                }
                else
                {
                    return BadRequest("Password errata.");
                }
            }
            else
            {
                return BadRequest("Utente non trovato.");
            }
        }

        private string GenerateSessionToken()
        {
            // Genera un token di sessione univoco e sicuro
            Guid sessionToken = Guid.NewGuid();
            return sessionToken.ToString();
        }



        public string HashPassword(string password)
        {
            // Genera un sale casuale
            byte[] salt;
            new RNGCryptoServiceProvider().GetBytes(salt = new byte[16]);

            // Crea l'hash della password usando PBKDF2
            var pbkdf2 = new Rfc2898DeriveBytes(password, salt, 10000);
            byte[] hash = pbkdf2.GetBytes(20);

            // Combina sale e hash
            byte[] hashBytes = new byte[36];
            Array.Copy(salt, 0, hashBytes, 0, 16);
            Array.Copy(hash, 0, hashBytes, 16, 20);

            // Converte in stringa base64
            string savedPasswordHash = Convert.ToBase64String(hashBytes);
            return savedPasswordHash;
        }


        public bool VerifyPassword(string enteredPassword, string savedPasswordHash)
        {
            // Converte l'hash salvato in un array di byte
            byte[] hashBytes = Convert.FromBase64String(savedPasswordHash);

            // Estrae il sale
            byte[] salt = new byte[16];
            Array.Copy(hashBytes, 0, salt, 0, 16);

            // Crea l'hash della password fornita utilizzando lo stesso sale
            var pbkdf2 = new Rfc2898DeriveBytes(enteredPassword, salt, 10000);
            byte[] hash = pbkdf2.GetBytes(20);

            // Confronta l'hash della password fornita con l'hash salvato
            for (int i = 0; i < 20; i++)
            {
                if (hashBytes[i + 16] != hash[i])
                {
                    return false;
                }
            }

            return true;
        }



        // POST api/values/register
        [HttpPost]
        [Route("api/values/register")]
        public IHttpActionResult Register([FromBody] Utenti user)

        {
            if (string.IsNullOrEmpty(user.Username) || string.IsNullOrEmpty(user.Psw))
            {
                return BadRequest("Username and password are required.");
            }

            // Hash the password
            string hashedPassword = HashPassword(user.Psw);

            // Create a new user
            Utenti newUser = new Utenti
            {
                Username = user.Username,
                Psw = hashedPassword
            };

            // Add the new user to the database
            TestDbContext db = new TestDbContext();
            db.Utenti.Add(newUser);
            db.SaveChanges();

            return Ok();
        }


    }
}
