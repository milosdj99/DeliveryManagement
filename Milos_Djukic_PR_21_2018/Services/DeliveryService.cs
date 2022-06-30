using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using Milos_Djukic_PR_21_2018.Configurations;
using Milos_Djukic_PR_21_2018.DTO;
using Milos_Djukic_PR_21_2018.Models;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Net;
using System.Net.Mail;
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
            else if (customer != null)
            {
                customer = _context.Customers.Where(x => x.Id == id).FirstOrDefault();
                user = _mapper.Map<UserRegisterDto>(customer);
                return user;
            }
            else
            {
                return null;
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


        public List<ArticleDTO> GetAllArticles()
        {
            var articles = _context.Articles.ToList();



            var articlesDtos = new List<ArticleDTO>();

            foreach (Article a in articles)
            {
                ArticleDTO adto = _mapper.Map<ArticleDTO>(a);
                articlesDtos.Add(adto);
            }

            return articlesDtos;
        }


        #region Customer

        public bool AddOrder(OrderDto order, Guid id)
        {

            if (_context.Orders.Where(x => x.CustomerId == id && x.Accepted == false).FirstOrDefault() != null)
            {
                return false;
            }

            Order newOrder = _mapper.Map<Order>(order);

            newOrder.Id = new Guid();

            newOrder.Accepted = false;

            newOrder.Time = newOrder.Time.AddDays(1);

            newOrder.Customer = _context.Customers.Where(x => x.Id == id).FirstOrDefault();

            newOrder.Deliverer = _context.Deliverers.Where(x => x.Id == new Guid("393f57b1-6b12-4b09-968f-08da5646ce5f")).FirstOrDefault();

            newOrder.OrderArticles = new List<OrderArticle>();

            foreach (ArticleDTO a in order.Articles)
            {
                Article arr = _context.Articles.Where(x => x.Id == a.Id).FirstOrDefault();


                newOrder.OrderArticles.Add(new OrderArticle() { Article = arr, Order = newOrder });
            }



            _context.Orders.Add(newOrder);

            _context.SaveChanges();

            return true;
        }

        public OrderDto GetCurrentOrderCustomer(Guid id)
        {
            var order = _context.Orders.Include(x => x.OrderArticles).ThenInclude(x => x.Article).Where(x => x.CustomerId == id && x.Time > DateTime.Now).FirstOrDefault();

            if (order == null)
            {
                return null;
            }

            var orderDto = _mapper.Map<OrderDto>(order);

            orderDto.Articles = new List<ArticleDTO>();

            foreach (OrderArticle oa in order.OrderArticles)
            {
                orderDto.Articles.Add(_mapper.Map<ArticleDTO>(oa.Article));
            }

            return orderDto;
        }

        public List<OrderDto> GetCustomerOrders(Guid id)
        {
            var orders = _context.Orders.Include(x => x.OrderArticles).ThenInclude(x => x.Article).Where(x => x.CustomerId == id && x.Time < DateTime.Now).ToList();

            return OrderListToDtos(orders);
        }

        #endregion


        #region Deliverer
        public List<OrderDto> GetNewOrdersDeliverer(Guid id)
        {
            var orders = _context.Orders.Include(x => x.OrderArticles).ThenInclude(x => x.Article).Where(x => x.Accepted == false).ToList();

            return OrderListToDtos(orders);
        }

        public List<OrderDto> GetDelivererOrders(Guid id)
        {
            var orders = _context.Orders.Include(x => x.OrderArticles).ThenInclude(x => x.Article).Where(x => x.DelivererId == id && x.Time < DateTime.Now).ToList();

            return OrderListToDtos(orders);
        }

        public OrderDto GetCurrentOrderDeliverer(Guid id)
        {
            var order = _context.Orders.Include(x => x.OrderArticles).ThenInclude(x => x.Article).Where(x => x.DelivererId == id && x.Accepted == true && x.Time > DateTime.Now).FirstOrDefault();

            if (order == null)
            {
                return null;
            }

            var orderDto = _mapper.Map<OrderDto>(order);

            orderDto.Articles = new List<ArticleDTO>();

            foreach (OrderArticle oa in order.OrderArticles)
            {
                orderDto.Articles.Add(_mapper.Map<ArticleDTO>(oa.Article));
            }

            return orderDto;
        }

        public bool ConfirmOrder(Guid id, Guid orderId)
        {

            if (_context.Orders.Where(x => x.DelivererId == id && x.Accepted == true && x.Time > DateTime.Now).FirstOrDefault() != null)
            {
                return false;
            }

            Deliverer user = _context.Deliverers.Where(x => x.Id == id).FirstOrDefault();

            _context.Orders.Where(x => x.Id == orderId).FirstOrDefault().Deliverer = user;

            _context.Orders.Where(x => x.Id == orderId).FirstOrDefault().Accepted = true;

            Random r = new Random();

            _context.Orders.Where(x => x.Id == orderId).FirstOrDefault().Time = DateTime.Now.AddMinutes(r.Next(1, 3));

            _context.SaveChanges();

            return true;
        }

        #endregion

        #region Admin

        public bool AddArticle(ArticleDTO article)
        {
            article.Id = new Guid();

            Article newArticle = _mapper.Map<Article>(article);

            if (_context.Articles.Where(x => x.Name == newArticle.Name).FirstOrDefault() != null)
            {
                return false;
            }

            _context.Articles.Add(newArticle);
            _context.SaveChanges();

            return true;
        }

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

        public List<OrderDto> GetAllOrders()
        {
            var orders = _context.Orders.Include(x => x.OrderArticles).ThenInclude(x => x.Article).ToList();

            return OrderListToDtos(orders);

        }

        #endregion

        #region Login/Register

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
                    issuer: "http://localhost:44312",
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

        public bool ChangePicture(Guid id, string path)
        {


            if (_context.Admins.Where(x => x.Id == id).FirstOrDefault() != null)
            {
                var user = _context.Admins.Where(x => x.Id == id).FirstOrDefault();
                user.ImageUrl = path;
                _context.SaveChanges();

                return true;
            }
            else if (_context.Deliverers.Where(x => x.Id == id).FirstOrDefault() != null)
            {
                var user = _context.Deliverers.Where(x => x.Id == id).FirstOrDefault();
                user.ImageUrl = path;
                _context.SaveChanges();

                return true;

            }
            else if (_context.Customers.Where(x => x.Id == id).FirstOrDefault() != null)
            {
                var user = _context.Customers.Where(x => x.Id == id).FirstOrDefault();
                user.ImageUrl = path;
                _context.SaveChanges();

                return true;

            }
            else
            {
                return false;
            }


        }


        public bool ModifyUser(Guid id, UserRegisterDto newUser)
        {

            if (_context.Deliverers.Where(x => x.Id == id).FirstOrDefault() == null && _context.Customers.Where(x => x.Id == id).FirstOrDefault() == null && _context.Admins.Where(x => x.Id == id).FirstOrDefault() == null)
            {
                return false;
            }

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
                    user1.State = "HOLD";

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
                issuer: "http://localhost:44312",
                claims: claims,
                expires: DateTime.Now.AddMinutes(20),
                signingCredentials: signinCredentials
            );
            string tokenString = new JwtSecurityTokenHandler().WriteToken(tokeOptions);
            return tokenString;
        }

        #endregion

        private List<OrderDto> OrderListToDtos(List<Order> list)
        {
            List<OrderDto> orderDtos = new List<OrderDto>();

            foreach (Order o in list)
            {
                OrderDto odto = _mapper.Map<OrderDto>(o);

                odto.Articles = new List<ArticleDTO>();

                foreach (OrderArticle a in o.OrderArticles)
                {

                    odto.Articles.Add(_mapper.Map<ArticleDTO>(a.Article));
                }

                orderDtos.Add(odto);
            }

            return orderDtos;
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
            catch (Exception e) {
                string ee = e.Message;
            }

        }
    }
}
