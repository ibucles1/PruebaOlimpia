using Microsoft.AspNetCore.Mvc;
using Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Text.RegularExpressions;
using System.Globalization;
using System.Text.Json;

namespace FacturaElectronica.Controllers
{
    [ApiController]
    [Route("api/validator")]
    public class ValidatorController : Controller
    {

        //[HttpGet("ValidateInvoiceList/{invoices}")]
        [HttpPost]
        [Route("ValidateInvoiceList")]
        public ActionResult ValidateInvoiceList(List<Invoice> invoices)
        {
            
            try
            {
                decimal total = 0;
                List<string> messages = new List<string>();

                if (invoices.Count() > 0)
                {
                    string result = "";

                    foreach (var invoice in invoices)
                    {
                        result = Validate(invoice);

                        if (invoices.Count(x => x.id == invoice.id) > 1)
                            result = result == "" ? "No pueden existir 2 facturas con el mismo Id." : result + ", No pueden existir 2 facturas con el mismo Id.";

                        if (result != "")
                            messages.Add("idFactura: " + invoice.id.ToString() + " - " + result);

                        total += invoice.totalValue;
                    }

                    if (messages.Count() > 0)
                        return BadRequest(JsonSerializer.Serialize(messages));
                    else
                        return Ok(JsonSerializer.Serialize(total));
                }
                else
                    return BadRequest("No hay elementos en la lista de facturas enviada.");
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message.ToString());
            }
        }

        //[HttpGet("ValidateInvoice/{invoice}")]
        [HttpPost]
        [Route("ValidateInvoice")]

        public ActionResult ValidateInvoice(Invoice invoice)
        {
            try
            {
                var result = Validate(invoice);

                if (result != "")
                    return BadRequest(result);

                return Ok(invoice.totalValue);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message.ToString());
            }


        }

        [HttpPost]
        [Route("CalculateIVA")]

        public ActionResult CalculateIVA(Invoice invoice)
        {
            try
            {
                var result = Validate(invoice);

                if (result == "")
                {

                    var iva = (invoice.totalValue * invoice.percentageIVA) / 100;

                    return Ok(iva);
                }
                else
                    return BadRequest(result);

            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message.ToString());
            }
        }


        private string Validate(Invoice invoice)
        {
            string message = "";

            if (invoice.id <= 0)
                message = "El id debe ser un numero entero positivo";

            if (!Regex.IsMatch(invoice.nit, "^[0-9]+$"))
                message = message == "" ?  "El NIT debe contener solo valores numericos" : message + ", El NIT debe contener solo valores numericos";

            if (!(invoice.percentageIVA >= 0 && invoice.percentageIVA <= 100))
                message = message == "" ? "El porcentaje debe ser un valor entre 0 y 100." : message + ", El porcentaje debe ser un valor entre 0 y 100";

            if (invoice.totalValue <= 0)
                message = message == "" ? "El valor total de la factura debe ser un numero entero positivo" : message + ", El valor total de la factura debe ser un numero entero positivo";
            

            return message;
        }       
    }
}
