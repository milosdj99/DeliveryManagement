using AutoMapper;
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

        public void AddArticle(ArticleDTO article)
        {
            Article newArticle = _mapper.Map<Article>(article);

            _context.Articles.Add(newArticle);
            _context.SaveChanges();
        }

        public void AddOrder(OrderDto order, Guid id)
        {
            Order newOrder = _mapper.Map<Order>(order);

            newOrder.Accepted = false;

            newOrder.Customer = _context.Customers.Where(x => x.Id == id).FirstOrDefault();

            newOrder.Deliverer = _context.Deliverers.Where(x => x.Id == new Guid("42E752EB-FAFE-40CF-26B1-08DA386D1DF1")).FirstOrDefault();

            newOrder.OrderArticles = new List<OrderArticle>();

            foreach(ArticleDTO a in order.Articles)
            {
                Article arr = _context.Articles.Where(x => x.Id == a.Id).FirstOrDefault();

                
                newOrder.OrderArticles.Add(new OrderArticle() { Article = arr, Order = newOrder });
            } 

            

            _context.Orders.Add(newOrder);

            _context.SaveChanges();
        }

        public OrderDto GetCurrentOrderCustomer(Guid id)
        {
            var order =  _context.Orders.Include(x => x.OrderArticles).ThenInclude(x => x.Article).Where(x => x.CustomerId == id && x.Accepted == true).FirstOrDefault();

            if(order == null)
            {
                return null;
            }

            var orderDto = _mapper.Map<OrderDto>(order);

            orderDto.Articles = new List<ArticleDTO>();

            foreach(OrderArticle oa in order.OrderArticles)
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
            var order = _context.Orders.Include(x => x.OrderArticles).ThenInclude(x => x.Article).Where(x => x.DelivererId == id && x.Accepted == true).FirstOrDefault();

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

        public void ConfirmOrder(Guid id, Guid orderId)
        {
            Deliverer user = _context.Deliverers.Where(x => x.Id == id).FirstOrDefault();

            _context.Orders.Where(x => x.Id == id).FirstOrDefault().Deliverer = user;

            _context.Orders.Where(x => x.Id == id).FirstOrDefault().Accepted = true;

            Random r = new Random();

            _context.Orders.Where(x => x.Id == id).FirstOrDefault().Time.AddMinutes(r.Next(1, 3));

            _context.SaveChanges();
        }


        


        public UserRegisterDto Register(UserRegisterDto userFromFront)
        {
            if (_context.Deliverers.Where(x => x.Username == userFromFront.Username).FirstOrDefault() != null || _context.Customers.Where(x => x.Username == userFromFront.Username).FirstOrDefault() != null)
            {
                return null;
            }

            if (userFromFront.Type == "Deliverer")
            {
                Deliverer user = _mapper.Map<Deliverer>(userFromFront);

                 user.Password = BCrypt.Net.BCrypt.HashPassword(user.Password);
                 _context.Deliverers.Add(user);
                 _context.SaveChanges();

                 return _mapper.Map<UserRegisterDto>(user);
                
            } else
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
                    claims.Add(new Claim("role", "admin"));
                    claims.Add(new Claim("id", AdminFromDatabase.Id.ToString()));
                }
                if (type == 2) {
                    claims.Add(new Claim(ClaimTypes.Role, "deliverer"));
                    claims.Add(new Claim("role", "deliverer"));
                    claims.Add(new Claim("id", DelivererFromDatabase.Id.ToString()));
                }
                if (type == 3) {
                    claims.Add(new Claim(ClaimTypes.Role, "customer"));
                    claims.Add(new Claim("role", "customer"));
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
    }
}
