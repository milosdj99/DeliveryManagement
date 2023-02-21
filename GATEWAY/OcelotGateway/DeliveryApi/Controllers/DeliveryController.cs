using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient.Server;
using DeliveryApi.Dto;
using DeliveryApi.Models;
using DeliveryApi.Services;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using System.Threading;

namespace DeliveryApi.Controllers
{
    [Route("api/delivery")]
    [ApiController]
    public class DeliveryController : ControllerBase
    {
        private readonly DeliveryService _service;
        public DeliveryController(DeliveryService service)
        {
            _service = service;
        }

        


        [HttpGet("articles")]
        public IActionResult GetArticles()
        {
            return Ok(_service.GetAllArticles());
        }


        #region Customer


        [Authorize(Roles = "customer")]
        [HttpPost("customer/add-order/{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public IActionResult AddOrder([FromBody] OrderDto order, [FromRoute] Guid id)
        {
            if (_service.AddOrder(order, id))
            {
                return Ok();
            }
            else
            {
                return BadRequest();
            }

        }


        [Authorize(Roles = "customer")]
        [HttpGet("customer/current-order/{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public IActionResult GetCurrentOrderCustomer([FromRoute] Guid id)
        {

            return Ok(_service.GetCurrentOrderCustomer(id));

        }

        [Authorize(Roles = "customer")]
        [HttpGet("customer/previous-orders/{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public IActionResult CustomerOrders([FromRoute] Guid id)
        {
            return Ok(_service.GetCustomerOrders(id));
        }

        #endregion

        #region Deliverer

        [Authorize(Roles = "deliverer")]
        [HttpGet("deliverer/new-orders/{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public IActionResult GetNewOrders(Guid id)
        {
            return Ok(_service.GetNewOrdersDeliverer(id));
        }

        [Authorize(Roles = "deliverer")]
        [HttpGet("deliverer/previous-orders/{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public IActionResult GetDelivererOrders(Guid id)
        {
            return Ok(_service.GetDelivererOrders(id));
        }

        [Authorize(Roles = "deliverer")]
        [HttpGet("deliverer/current-order/{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public IActionResult GetCurrentOrderDeliverer(Guid id)
        {
            return Ok(_service.GetCurrentOrderDeliverer(id));
        }

        [Authorize(Roles = "deliverer")]
        [HttpGet("deliverer/confirm-order/{id}/{orderId}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public IActionResult ConfirmOrder(Guid id, Guid orderId)
        {
            Thread.BeginCriticalRegion();

            string s = _service.ConfirmOrder(id, orderId);

            Thread.EndCriticalRegion();

            if (s == "Ok")
            {
                return Ok();
            }
            else
            {
                return BadRequest(s);
            }

            
        }

        #endregion


        #region Admin


        [Authorize(Roles = "admin")]
        [HttpPost("admin/add-article")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public IActionResult AddArticle([FromBody] ArticleDTO article)
        {
            if (_service.AddArticle(article))
            {
                return Ok();
            }
            else
            {
                return BadRequest();
            }


        }
           

        [Authorize(Roles = "admin")]
        [HttpGet("admin/all-orders")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public IActionResult GetAllOrders()
        {
            return Ok(_service.GetAllOrders());
        }



        #endregion


    }
}
