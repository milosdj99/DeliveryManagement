using AutoMapper;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using Milos_Djukic_PR_21_2018.Configurations;
using Milos_Djukic_PR_21_2018.DTO;
using Milos_Djukic_PR_21_2018.Models;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace Milos_Djukic_PR_21_2018.Services
{
    public class DeliveryService
    {
        private readonly DeliveryDbContext _context;

        private readonly IMapper _mapper;

        private readonly string _secretKey;

        public DeliveryService(DeliveryDbContext context, IConfiguration config, IMapper mapper)
        {
            _context = context;
            _secretKey = config.GetSection("SecretKey").Value;
            _mapper = mapper;
        }

        public List<Deliverer> GetAllDeliverers()
        {
            return _context.Deliverers.ToList();
        }

        public UserRegisterDto GetUserById(Guid id)
        {
            var user = new UserRegisterDto();
            var deliverer = new Deliverer();
            var customer = new Customer();

            deliverer = _context.Deliverers.Where(x => x.Id == id).FirstOrDefault();
            customer = _context.Customers.Where(x => x.Id == id).FirstOrDefault();

            if (deliverer != null)
            {
                user = _mapper.Map<UserRegisterDto>(deliverer);
                return user;
            }
            else if(customer != null)
            {
                customer = _context.Customers.Where(x => x.Id == id).FirstOrDefault();
                user = _mapper.Map<UserRegisterDto>(customer);
                return user;
            } else
            {
                return null;
            }
        }

        public void ModifyUser(Guid id, UserRegisterDto newUser)
        {
            var user = _context.Admins.Where(x => x.Id == newUser.Id).FirstOrDefault();

            if (user != null)
            {
                user.Username = newUser.Username;
                user.Name = newUser.Name;
                user.Surname = newUser.Surname;
                user.DateOfBirth = newUser.DateOfBirth;
                user.Address = newUser.Address;
                user.Email = newUser.Email;
                user.ImageUrl = newUser.ImageUrl;

                _context.SaveChanges();

            }
            else
            {
                var user1 = _context.Deliverers.Where(x => x.Id == newUser.Id).FirstOrDefault();

                if (user1 != null)
                {
                    user1.Username = newUser.Username;
                    user1.Name = newUser.Name;
                    user1.Surname = newUser.Surname;
                    user1.DateOfBirth = newUser.DateOfBirth;
                    user1.Address = newUser.Address;
                    user1.Email = newUser.Email;
                    user1.ImageUrl = newUser.ImageUrl;

                    _context.SaveChanges();

                }
                else
                {

                    var user2 = _context.Deliverers.Where(x => x.Id == newUser.Id).FirstOrDefault();

                    user2.Username = newUser.Username;
                    user2.Name = newUser.Name;
                    user2.Surname = newUser.Surname;
                    user2.DateOfBirth = newUser.DateOfBirth;
                    user2.Address = newUser.Address;
                    user2.Email = newUser.Email;
                    user2.ImageUrl = newUser.ImageUrl;

                    _context.SaveChanges();
                }

                

            }
        }

        public UserRegisterDto GetUserByUsername(string username)
        {
            var user = new UserRegisterDto();
            var deliverer = new Deliverer();
            var customer = new Customer();

            deliverer = _context.Deliverers.Where(x => x.Username == username).FirstOrDefault();
            customer = _context.Customers.Where(x => x.Username == username).FirstOrDefault();

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
            else
            {
                return null;
            }
        }

        public UserRegisterDto Register(UserRegisterDto userFromFront)
        {
            

            if(userFromFront.Type == "Deliverer")
            {
                Deliverer user = _mapper.Map<Deliverer>(userFromFront);


                if(_context.Deliverers.Where(x => x.Username == userFromFront.Username).FirstOrDefault() != null)
                {
                    return null;
                } else
                {
                    user.Password = BCrypt.Net.BCrypt.HashPassword(user.Password);
                    _context.Deliverers.Add(user);
                    _context.SaveChanges();

                    return _mapper.Map<UserRegisterDto>(user);
                }



            } else
            {
                Customer user = _mapper.Map<Customer>(userFromFront);


                if (_context.Customers.Where(x => x.Username == userFromFront.Username).FirstOrDefault() != null)
                {
                    return null;
                }
                else
                {
                    user.Password = BCrypt.Net.BCrypt.HashPassword(user.Password);
                    _context.Customers.Add(user);
                    _context.SaveChanges();

                    return _mapper.Map<UserRegisterDto>(user);
                }
            }
        }

        public string Login(UserLoginDto user)
        {
            int type;
            Admin AdminFromDatabase = null;
            Deliverer DelivererFromDatabase = null;
            Customer CustomerFromDatabase = null;

            string hashedPassword;

            if(_context.Admins.Where(x => x.Username == user.Username).FirstOrDefault() != null)
            {
                AdminFromDatabase = _context.Admins.Where(x => x.Username == user.Username).FirstOrDefault();

                type = 1;

                hashedPassword = AdminFromDatabase.Password;
            }
            else if (_context.Deliverers.Where(x => x.Username == user.Username).FirstOrDefault() != null)
            {
                DelivererFromDatabase = _context.Deliverers.Where(x => x.Username == user.Username).FirstOrDefault();

                type = 2;

                hashedPassword = DelivererFromDatabase.Password;
            }
            else if (_context.Customers.Where(x => x.Username == user.Username).FirstOrDefault() != null)
            {
                CustomerFromDatabase = _context.Customers.Where(x => x.Username == user.Username).FirstOrDefault();

                type = 3;

                hashedPassword = CustomerFromDatabase.Password;
            } else
            {
                return null;
            }

            

            if (BCrypt.Net.BCrypt.Verify(user.Password, hashedPassword))
            {
                List<Claim> claims = new List<Claim>();

                if (type == 1) {
                    claims.Add(new Claim(ClaimTypes.Role, "admin"));
                    claims.Add(new Claim("id", AdminFromDatabase.Id.ToString()));
                }
                if (type == 2) {
                    claims.Add(new Claim(ClaimTypes.Role, "deliverer"));
                    claims.Add(new Claim("id", DelivererFromDatabase.Id.ToString()));
                }
                if (type == 3) {
                    claims.Add(new Claim(ClaimTypes.Role, "customer"));
                    claims.Add(new Claim("id", CustomerFromDatabase.Id.ToString()));
                }

                

                
                SymmetricSecurityKey secretKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_secretKey));
                var signinCredentials = new SigningCredentials(secretKey, SecurityAlgorithms.HmacSha256);
                var tokeOptions = new JwtSecurityToken(
                    issuer: "http://localhost:44398", 
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
    }
}
