﻿using FBus.Business.BaseBusiness.CommonModel;
using Microsoft.AspNetCore.Mvc;
using System;

namespace FBus.API.Controllers
{
    public class BaseController : ControllerBase
    {
        [NonAction]
        public ObjectResult SendResponse(object result)
        {
            int statusCode = 200;
            string message = "Thành công";
            if (result == null)
            {
                statusCode = 404;
                message = "Không tìm thấy";
            }
            else if (result.GetType().GetProperties()[0].Name == "Id" && (result.GetType().GetProperties()[0].GetValue(result)).ToString() == "-1")
            {
                statusCode = 404;
                message = "Không thể tìm thấy xe trên bản đồ";
                result = null;
            }
            return HandleObjectResponse(statusCode, message, result);
        }

        [NonAction]
        public ObjectResult SendResponse(bool result)
        {
            int statusCode = result ? 201 : 400;
            string message = result ? "Thành công" : "Dữ liệu không phù hợp";
            return HandleObjectResponse(statusCode, message, null);
        }

        [NonAction]
        public ObjectResult SendResponse(Response response)
        {
            return HandleObjectResponse(response.StatusCode, response.Message, response.Data);
        }

        [NonAction]
        public ObjectResult ResponseOTP(Response response)
        {
            return HandleObjectResponse(response.StatusCode, response.Message, response.Data);
        }

        private ObjectResult HandleObjectResponse(int statusCode, string message, object result)
        {
            ObjectResult objectResult = null;
            Object responseData = null;

            if
            (result == null)
            {
                responseData = new
                {
                    statusCode = statusCode,
                    message = message
                };
            }
            else
            {
                responseData = new
                {
                    statusCode = statusCode,
                    message = message,
                    body = result
                };
            }

            objectResult = new ObjectResult(responseData);
            objectResult.StatusCode = statusCode;
            return objectResult;
        }
    }
}