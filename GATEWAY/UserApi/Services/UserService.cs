using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http.Headers;
using System.Net.Mail;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using UserApi.Dto;
using UserApi.Infrastructure;
using UserApi.Models;

namespace UserApi
{
    public class UserService
    {



        private readonly UserDbContext _context;

        private readonly IMapper _mapper;

        private readonly string _secretKey;

        public UserService(UserDbContext context, IConfiguration config, IMapper mapper)
        {
            _context = context;
            _secretKey = config.GetSection("SecretKey").Value;
            _mapper = mapper;
        }



        public UserRegisterDto Register(UserRegisterDto userFromFront)
        {


            if (_context.Deliverers.Where(x => x.Email == userFromFront.Email).FirstOrDefault() != null || _context.Customers.Where(x => x.Email == userFromFront.Email).FirstOrDefault() != null || _context.Admins.Where(x => x.Email == userFromFront.Email).FirstOrDefault() != null)
            {
                return null;
            }
           

            if (userFromFront.Type == "Deliverer")
            {
                Deliverer user = _mapper.Map<Deliverer>(userFromFront);
                user.State = "HOLD";
                user.Password = BCrypt.Net.BCrypt.HashPassword(user.Password);
                _context.Deliverers.Add(user);
                _context.SaveChanges();

                return _mapper.Map<UserRegisterDto>(user);

            }
            else
            {
                Customer user = _mapper.Map<Customer>(userFromFront);

                user.Password = BCrypt.Net.BCrypt.HashPassword(user.Password);
                _context.Customers.Add(user);
                _context.SaveChanges();



                return _mapper.Map<UserRegisterDto>(user);


            } 
        }





        public string Login(UserLoginDto user)
        {
            int type;
            Admin AdminFromDatabase = null;
            Deliverer DelivererFromDatabase = null;
            Customer CustomerFromDatabase = null;

            string hashedPassword;

            if (_context.Admins.Where(x => x.Email == user.Email).FirstOrDefault() != null)
            {
                AdminFromDatabase = _context.Admins.Where(x => x.Email == user.Email).FirstOrDefault();

                type = 1;

                hashedPassword = AdminFromDatabase.Password;
            }
            else if (_context.Deliverers.Where(x => x.Email == user.Email).FirstOrDefault() != null)
            {
                DelivererFromDatabase = _context.Deliverers.Where(x => x.Email == user.Email).FirstOrDefault();

                type = 2;

                hashedPassword = DelivererFromDatabase.Password;
            }
            else if (_context.Customers.Where(x => x.Email == user.Email).FirstOrDefault() != null)
            {
                CustomerFromDatabase = _context.Customers.Where(x => x.Email == user.Email).FirstOrDefault();

                type = 3;

                hashedPassword = CustomerFromDatabase.Password;
            }
            else
            {
                return null;
            }



            if (BCrypt.Net.BCrypt.Verify(user.Password, hashedPassword))
            {
                List<Claim> claims = new List<Claim>();

                if (type == 1)
                {
                    claims.Add(new Claim(ClaimTypes.Role, "admin"));
                    claims.Add(new Claim("role", "admin"));
                    claims.Add(new Claim("id", AdminFromDatabase.Id.ToString()));
                }
                if (type == 2)
                {
                    if (DelivererFromDatabase.State != "APPROVED")
                    {
                        return "NOT_APPROVED";
                    }
                    claims.Add(new Claim(ClaimTypes.Role, "deliverer"));
                    claims.Add(new Claim("role", "deliverer"));
                    claims.Add(new Claim("id", DelivererFromDatabase.Id.ToString()));
                }
                if (type == 3)
                {
                    claims.Add(new Claim(ClaimTypes.Role, "customer"));
                    claims.Add(new Claim("role", "customer"));
                    claims.Add(new Claim("id", CustomerFromDatabase.Id.ToString()));
                }




                SymmetricSecurityKey secretKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_secretKey));
                var signinCredentials = new SigningCredentials(secretKey, SecurityAlgorithms.HmacSha256);
                var tokeOptions = new JwtSecurityToken(
                    issuer: "http://localhost:31210",
                    claims: claims,
                    expires: DateTime.Now.AddMinutes(20),
                    signingCredentials: signinCredentials
                );
                string tokenString = new JwtSecurityTokenHandler().WriteToken(tokeOptions);
                return tokenString;
            }
            else
            {
                return null;
            }


        }

        public bool ChangePicture(Guid id, IFormFile file)
        {
            var folderName = Path.Combine("Resources", "Images");

            var path = Path.Combine(Directory.GetCurrentDirectory(), folderName);

            if (file.Length <= 0)
            {
                return false;
            }

            var fileName = ContentDispositionHeaderValue.Parse(file.ContentDisposition).FileName.Trim('"');
            var fullPath = Path.Combine(path, fileName);
            var dbPath = Path.Combine(folderName, fileName);

            using (var stream = new FileStream(fullPath, FileMode.Create))
            {
                file.CopyTo(stream);
            }



            if (_context.Admins.Where(x => x.Id == id).FirstOrDefault() != null)
            {
                var user = _context.Admins.Where(x => x.Id == id).FirstOrDefault();
                user.ImageUrl = dbPath;
                _context.SaveChanges();

                return true;
            }
            else if (_context.Deliverers.Where(x => x.Id == id).FirstOrDefault() != null)
            {
                var user = _context.Deliverers.Where(x => x.Id == id).FirstOrDefault();
                user.ImageUrl = dbPath;
                _context.SaveChanges();

                return true;

            }
            else if (_context.Customers.Where(x => x.Id == id).FirstOrDefault() != null)
            {
                var user = _context.Customers.Where(x => x.Id == id).FirstOrDefault();
                user.ImageUrl = dbPath;
                _context.SaveChanges();

                return true;

            }
            else
            {
                return false;
            }


        }


        public string GetPicture(Guid id)
        {
            UserRegisterDto user = GetUserById(id);

            if(user.ImageUrl == null)
            {
                return "";

            } else
            {
                byte[] byteArrImg = File.ReadAllBytes(user.ImageUrl);
                return Convert.ToBase64String(byteArrImg);               
            }
           
        }


        public bool ModifyUser(Guid id, UserRegisterDto newUser)
        {

            if (_context.Deliverers.Where(x => x.Id == id).FirstOrDefault() == null && _context.Customers.Where(x => x.Id == id).FirstOrDefault() == null && _context.Admins.Where(x => x.Id == id).FirstOrDefault() == null)
            {
                return false;
            }

            if (_context.Deliverers.Where(x => x.Email == newUser.Email && x.Id != id).FirstOrDefault() != null || _context.Customers.Where(x => x.Email == newUser.Email && x.Id != id).FirstOrDefault() != null || _context.Admins.Where(x => x.Email == newUser.Email && x.Id != id).FirstOrDefault() != null)
            {
                return false;
            }

            var user = _context.Admins.Where(x => x.Id == id).FirstOrDefault();

            if (user != null)
            {
                user.Username = newUser.Username;
                user.Name = newUser.Name;
                user.Surname = newUser.Surname;
                user.DateOfBirth = newUser.DateOfBirth;
                user.Address = newUser.Address;
                user.Email = newUser.Email;
                user.Password = BCrypt.Net.BCrypt.HashPassword(newUser.Password);

                _context.SaveChanges();

            }
            else
            {
                var user1 = _context.Deliverers.Where(x => x.Id == id).FirstOrDefault();

                if (user1 != null)
                {
                    user1.Username = newUser.Username;
                    user1.Name = newUser.Name;
                    user1.Surname = newUser.Surname;
                    user1.DateOfBirth = newUser.DateOfBirth;
                    user1.Address = newUser.Address;
                    user1.Email = newUser.Email;
                    user1.Password = BCrypt.Net.BCrypt.HashPassword(newUser.Password);


                    _context.SaveChanges();

                }
                else
                {

                    var user2 = _context.Customers.Where(x => x.Id == id).FirstOrDefault();

                    user2.Username = newUser.Username;
                    user2.Name = newUser.Name;
                    user2.Surname = newUser.Surname;
                    user2.DateOfBirth = newUser.DateOfBirth;
                    user2.Address = newUser.Address;
                    user2.Email = newUser.Email;
                    user2.Password = BCrypt.Net.BCrypt.HashPassword(newUser.Password);

                    _context.SaveChanges();
                }



            }

            return true;
        }

        public string FacebookLogin(UserRegisterDto user)
        {
            user.Id = new Guid();
            Customer customer = _mapper.Map<Customer>(user);

            Customer customerFromDatabase = _context.Customers.Where(x => x.Email == customer.Email).FirstOrDefault();


            if (customerFromDatabase == null)   //ne postoji
            {
                _context.Customers.Add(customer);
                _context.SaveChanges();

            }
            else
            {
                customer = customerFromDatabase;
            }

            List<Claim> claims = new List<Claim>();

            claims.Add(new Claim(ClaimTypes.Role, "customer"));
            claims.Add(new Claim("role", "customer"));
            claims.Add(new Claim("id", customer.Id.ToString()));


            SymmetricSecurityKey secretKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_secretKey));
            var signinCredentials = new SigningCredentials(secretKey, SecurityAlgorithms.HmacSha256);
            var tokeOptions = new JwtSecurityToken(
                issuer: "http://localhost:31210",
                claims: claims,
                expires: DateTime.Now.AddMinutes(20),
                signingCredentials: signinCredentials
            );
            string tokenString = new JwtSecurityTokenHandler().WriteToken(tokeOptions);
            return tokenString;
        }




        public UserRegisterDto GetUserById(Guid id)
        {
            var user = new UserRegisterDto();
            var deliverer = new Deliverer();
            var customer = new Customer();
            var admin = new Admin();

            deliverer = _context.Deliverers.Where(x => x.Id == id).FirstOrDefault();
            customer = _context.Customers.Where(x => x.Id == id).FirstOrDefault();
            admin = _context.Admins.Where(x => x.Id == id).FirstOrDefault();

            if (deliverer != null)
            {
                user = _mapper.Map<UserRegisterDto>(deliverer);
                return user;
            }
            else if (customer != null)
            {
                
                user = _mapper.Map<UserRegisterDto>(customer);
                return user;
            }
            else if (admin != null)
            {

                user = _mapper.Map<UserRegisterDto>(admin);
                return user;
            }
            else
            {
                return null;
            }
        }

        #region Admin


        public List<UserRegisterDto> GetAllDeliverers()
        {
            var deliverers = _context.Deliverers.ToList();

            List<UserRegisterDto> delivererDtos = new List<UserRegisterDto>();

            foreach (Deliverer d in deliverers)
            {
                UserRegisterDto dto = _mapper.Map<UserRegisterDto>(d);

                delivererDtos.Add(dto);
            }

            return delivererDtos;
        }

        public void ChangeState(Guid id, string state)
        {
            var user = _context.Deliverers.Where(x => x.Id == id).FirstOrDefault();

            user.State = state;

            Email(user.Email, state);

            _context.SaveChanges();
        }


        private void Email(string to, string state)
        {
            string statee = "";

            switch (state)
            {
                case "APPROVED":
                    statee = "ODOBREN";
                    break;
                case "DENIED":
                    statee = "ODBIJEN";
                    break;
                case "HOLD":
                    statee = "NA CEKANJU";
                    break;
            }

            try
            {
                MailMessage message = new MailMessage();
                SmtpClient smtp = new SmtpClient();
                message.From = new MailAddress("milosdj9999@gmail.com");
                message.To.Add(new MailAddress(to));
                message.Subject = "Promena stanja dostavljaca";
                message.IsBodyHtml = false; //to make message body as html  
                message.Body = $"Vase stanje kao dostavljaca je promenjeno na {statee}.";
                smtp.Port = 587;
                smtp.Host = "smtp.gmail.com"; //for gmail host  
                smtp.EnableSsl = true;
                smtp.UseDefaultCredentials = false;
                smtp.Credentials = new NetworkCredential("milosdj9999@gmail.com", "reqrtgqjecxaojxd");
                smtp.DeliveryMethod = SmtpDeliveryMethod.Network;
                smtp.Send(message);
            }
            catch (Exception e)
            {
                string ee = e.Message;
            }

        }

        #endregion


        #region Helper

        public UserRegisterDto GetUserByEmail(string email)
        {
            var user = new UserRegisterDto();
            var deliverer = new Deliverer();
            var customer = new Customer();
            var admin = new Admin();

            deliverer = _context.Deliverers.Where(x => x.Email == email).FirstOrDefault();
            customer = _context.Customers.Where(x => x.Email == email).FirstOrDefault();
            admin = _context.Admins.Where(x => x.Email == email).FirstOrDefault();

            if (deliverer != null)
            {
                user = _mapper.Map<UserRegisterDto>(deliverer);
                return user;
            }
            else if (customer != null)
            {

                user = _mapper.Map<UserRegisterDto>(customer);
                return user;
            }
            else if (admin != null)
            {

                user = _mapper.Map<UserRegisterDto>(admin);
                return user;
            }
            else
            {
                return null;
            }
        }

        #endregion
    }
}
