using AutoWrapper.Wrappers;
using System;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using customerportalapi.Services.Exceptions;
using customerportalapi.Services.Interfaces;
using customerportalapi.Security;
using customerportalapi.Entities;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace customerportalapi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [AuthorizeToken]
    public class EkomiWidgetController : ControllerBase
    {
        private readonly IEkomiWidgetService _service;
        private readonly ILogger<ProcessController> _logger;

        public EkomiWidgetController(IEkomiWidgetService service, ILogger<ProcessController> logger)
        {
            _service = service;
            _logger = logger;
        }

        /// <summary>
        /// Get an Ekomi Widget 
        /// </summary>
        /// <param name="storeCode">siteId</param>
        /// <returns>Ekomiwidget data</returns>
        /// <remarks>This method searches from the database the EkomiWidget with the storeCode</remarks>
        /// <response code = "200">Return an EkomiWidget</response>
        /// <response code = "500">Internal Server Error</response>
        [HttpGet("{storeCode}/{ekomiLanguage}")]
        public ApiResponse GetEkomiWidget(string storeCode)
        {
            try
            {
                var result = _service.GetEkomiWidget(storeCode);
                return new ApiResponse(result);
            }
            catch (ServiceException se)
            {
                _logger.LogError(se.ToString());
                return new ApiResponse((int)se.StatusCode, new ApiError(se.Message, new[] { new ValidationError(se.Field, se.FieldMessage) }));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.ToString());
                throw;
            }
        }

        /// <summary>
        /// Create new ekomiWidget
        /// </summary>
        /// <param name="ekomiWidget">Ekomiwidget</param>
        /// <returns></returns>
        /// <remarks>This method inserts into the databse an EkomiWidget
        /// 
        /// </remarks>
        [HttpPost]
        public ApiResponse CreateEkomiWidget(EkomiWidget ekomiWidget)
        {
            try
            {
                var result = _service.CreateEkomiWidget(ekomiWidget);
                return new ApiResponse(result);
            }
            catch (ServiceException se)
            {
                string obj = string.Empty;
                if (ekomiWidget != null)
                    obj = ", params:" + JsonConvert.SerializeObject(ekomiWidget);

                _logger.LogError(se, se.Message + obj);
                return new ApiResponse((int)se.StatusCode, new ApiError(se.Message, new[] { new ValidationError(se.Field, se.FieldMessage) }));
            }
            catch (Exception ex)
            {
                string obj = string.Empty;
                if (ekomiWidget != null)
                    obj = ", params:" + JsonConvert.SerializeObject(ekomiWidget);

                _logger.LogError(ex, ex.Message + obj);
                throw;
            }
        }

        /// <summary>
        /// Create new multiple ekomiWidgets
        /// </summary>
        /// <param name="ekomkiWidgets">Ekomiwidget List</param>
        /// <returns></returns>
        [HttpPost("multiple")]
        public ApiResponse CreateMultipleEkomiWidget(List<EkomiWidget> ekomiWidgets)
        {
            try
            {
                var result = _service.CreateMultipleEkomiWidgets(ekomiWidgets);
                return new ApiResponse(result);
            }
            catch (ServiceException se)
            {
                string obj = string.Empty;
                if (ekomiWidgets != null)
                    obj = ", params:" + JsonConvert.SerializeObject(ekomiWidgets);

                _logger.LogError(se, se.Message + obj);
                return new ApiResponse((int)se.StatusCode, new ApiError(se.Message, new[] { new ValidationError(se.Field, se.FieldMessage) }));
            }
            catch (Exception ex)
            {
                string obj = string.Empty;
                if (ekomiWidgets != null)
                    obj = ", params:" + JsonConvert.SerializeObject(ekomiWidgets);

                _logger.LogError(ex, ex.Message + obj);
                throw;
            }
        }

        /// <summary>
        /// Update ekomiWidget
        /// </summary>
        /// <param name="ekomkiWidget">Ekomiwidget</param>
        /// <returns>EkomiWidget</returns>
        [HttpPut]
        public ApiResponse UpdateEkomiWidget(EkomiWidget ekomiWidget)
        {
            try
            {
                var result = _service.UpdateEkomiWidget(ekomiWidget);
                return new ApiResponse(result);
            }
            catch (ServiceException se)
            {
                string obj = string.Empty;
                if (ekomiWidget != null)
                    obj = ", params:" + JsonConvert.SerializeObject(ekomiWidget);

                _logger.LogError(se, se.Message + obj);
                return new ApiResponse((int)se.StatusCode, new ApiError(se.Message, new[] { new ValidationError(se.Field, se.FieldMessage) }));
            }
            catch (Exception ex)
            {
                string obj = string.Empty;
                if (ekomiWidget != null)
                    obj = ", params:" + JsonConvert.SerializeObject(ekomiWidget);

                _logger.LogError(ex, ex.Message + obj);
                throw;
            }
        }

        /// <summary>
        /// Delete ekomiWidget
        /// </summary>
        /// <param name="id">Ekomiwidget id</param>
        /// <returns></returns>
        [HttpDelete("{id}")]
        public ApiResponse DeleteEkomiWidget(string id)
        {
            try
            {
                var result = _service.DeleteEkomiWidget(id);
                return new ApiResponse(result);
            }
            catch (ServiceException se)
            {
                _logger.LogError(se.ToString());
                return new ApiResponse((int)se.StatusCode, new ApiError(se.Message, new[] { new ValidationError(se.Field, se.FieldMessage) }));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.ToString());
                throw;
            }
        }

        /// <summary>
        /// Find ekomiWidget
        /// </summary>
        /// <param name="ekomiWidgetSearchFilter">EkomiWidgetSearchFilter</param>
        /// <returns>List<EkomiWidgetSearchFilter></returns>
        [HttpPost("search")]
        public ApiResponse FindEkomiWidget(EkomiWidgetSearchFilter ekomiWidgetSearchFilter)
        {
            try
            {
                var result = _service.FindEkomiWidgets(ekomiWidgetSearchFilter);
                return new ApiResponse(result);
            }
            catch (ServiceException se)
            {
                _logger.LogError(se.ToString());
                return new ApiResponse((int)se.StatusCode, new ApiError(se.Message, new[] { new ValidationError(se.Field, se.FieldMessage) }));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.ToString());
                throw;
            }
        }
    }
}