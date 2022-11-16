﻿using AutoWrapper.Wrappers;
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
        /// Create new EkomiWidget
        /// </summary>
        /// <param name="ekomiWidget">Ekomiwidget</param>
        /// <returns>EkomiWidget</returns>
        /// <remarks>This method first checks if an EkomiWidget already exists in the database, if it doesn't it creates it
        /// <response code = "200">ekomiWidget created</response>
        /// <response code = "400">Handled error
        /// - SiteId required
        /// - EkomicustomerId required
        /// - EkomiWidgetTokens required
        /// - EkomiWidget exist with same siteId and EkomiLanguage please update
        /// </response>
        /// <response code = "500">Error of type Internal Server Error</response>
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
        /// <returns>EkomiWidget</returns>
        /// <remarks>This method check first if exits some ekomiWidget with same SiteId and Language otherwise it creates multiple ekomiWidgets</remarks>
        /// <response code= "200">Multiple ekomiWidgets created</response>
        /// <response code="400">Handled error: EkomiWidget exist with same siteId and EkomiLanguage please update</response>
        /// <response code= "500">Error of type Internal Server Error</response>
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
        /// <param name="ekomiWidget">Ekomiwidget</param>
        /// <returns>EkomiWidget</returns>
        /// <remarks>This method first checks in the database if the EkomiWidget exists, if it does
        /// it updates it with the information provided in the request.
        /// </remarks>
        /// <response code = "200">ekomiWidget updated</response>
        /// <response code="404">Handled Error: EkomiWidget by Id Not Found</response>
        /// <response code = "500">Error of type Internal Server Error</response>
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
        /// <returns>true</returns>
        /// <remarks>This method first checks in the database if the EkomiWidget exists, if it exists, it deletes the record of this.</remarks>
        /// <response code = "200">ekomiWidget deleted</response>
        /// <response code="400">Handled error: Id required</response>
        /// <response code = "500">Error of type Internal Server Error</response>
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
        /// <returns>List of EkomiWidgetSearchFilter</returns>
        /// <remarks>This method searches the database for an EkomiWidget with the filters of storeCode, EkomiWidgetTokens and EkomiCustomerId</remarks>
        /// <response code = "200">Return an ekomiWidget</response>
        /// <response code = "500">Error of type Internal Server Error</response>
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