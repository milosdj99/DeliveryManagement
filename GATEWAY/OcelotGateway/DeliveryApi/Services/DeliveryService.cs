using AutoMapper;
using DeliveryApi.Dto;
using DeliveryApi.Infrastructure;
using DeliveryApi.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DeliveryApi.Services
{
    public class DeliveryService
    {
        private readonly DeliveryDbContext _context;

        private readonly IMapper _mapper;
        

        public DeliveryService(DeliveryDbContext context, IConfiguration config, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
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

            newOrder.CustomerId = id;

            newOrder.DelivererId = new Guid("393f57b1-6b12-4b09-968f-08da5646ce5f");    //genericki

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

        public string ConfirmOrder(Guid id, Guid orderId)
        {

            if (_context.Orders.Where(x => x.Id == orderId && x.Accepted == true).FirstOrDefault() != null)          //vec je neko prihvatio
            {
                return "Porudzbina je vec prihvacena od strane drugog dostavljaca!";
            }


            if (_context.Orders.Where(x => x.DelivererId == id && x.Accepted == true && x.Time > DateTime.Now).FirstOrDefault() != null) //vec ima porudzbinu
            {
                return "Vec postoji porudzbina koju dostavljate!";
            }

            

            _context.Orders.Where(x => x.Id == orderId).FirstOrDefault().DelivererId = id;

            _context.Orders.Where(x => x.Id == orderId).FirstOrDefault().Accepted = true;

            Random r = new Random();

            _context.Orders.Where(x => x.Id == orderId).FirstOrDefault().Time = DateTime.Now.AddMinutes(r.Next(1, 3));

            _context.SaveChanges();

            return "Ok";
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

       

        

        public List<OrderDto> GetAllOrders()
        {
            var orders = _context.Orders.Include(x => x.OrderArticles).ThenInclude(x => x.Article).ToList();

            return OrderListToDtos(orders);

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

        
    }
}
