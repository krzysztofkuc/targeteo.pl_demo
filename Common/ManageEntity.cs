using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using targeteo.pl.Model;
using targeteo.pl.Services;

namespace targeteo.pl.Common
{
    public class ManageEntity
    {
        public static async void ChangeOrderStatus(ApplicationDbContext db,  string orderId, string changeToStatus, string trackingNo)
        {
            var order = db.OneUserOrder.Include(o => o.OrderDetail).ThenInclude(det => det.Product).FirstOrDefault(o => o.OrderSummary.Id == orderId);
            order.Status = changeToStatus;

            //get caption from string as enum
            var statusCaption = string.Empty;
            switch (changeToStatus)
            {
                case nameof(Enums.OrderStatus.sent):
                    statusCaption = "wysłano";
                    break;
                case nameof(Enums.OrderStatus.preparing):
                    statusCaption = "w przygotowaniu";
                    break;
                case nameof(Enums.OrderStatus.canceled):
                    foreach (var orderDetails in order.OrderDetail)
                    {
                        orderDetails.Product.QuantityInStock += orderDetails.Quantity;
                    }
                    statusCaption = "anulowano";
                    break;
                case nameof(Enums.OrderStatus.returnShipment):
                    statusCaption = "zwrócono";
                    break;
                case nameof(Enums.OrderStatus.waitingForPayment):
                    statusCaption = "oczekiwanie na płatność";
                    break;
                case nameof(Enums.OrderStatus.paymentCompleted):
                    statusCaption = "opłacono";
                    break;
                case nameof(Enums.OrderStatus.receiptConfirmed):
                    statusCaption = "potwierdzono odbiór";
                    break;
            }

            if (changeToStatus == Enums.OrderStatus.sent.ToString())
            {
                order.TrackingNo = trackingNo;
            }

            db.OneUserOrder.Update(order);
            db.SaveChanges();


            #region send change satus email
            if (changeToStatus == Enums.OrderStatus.sent.ToString())
            {

                var companyInfo = db.CompanyInformation.FirstOrDefault();

                await EmailService.SendAsync("Zamówienie: " + orderId + " - zmieniono status na: '" + statusCaption + "'", "Nr. listu przewozowego: " + trackingNo, order.Email, null, companyInfo);
                await EmailService.SendAsync("Zamówienie: " + orderId + " - zmieniono status na: '" + statusCaption + "'", "Nr. listu przewozowego: " + trackingNo, companyInfo.Email, null, companyInfo);
            }
            else
            {
                var companyInfo = db.CompanyInformation.FirstOrDefault();
                await EmailService.SendAsync("Zamówienie: " + orderId + " - zmieniono status na: '" + statusCaption + "'", string.Empty, order.Email, null, companyInfo);
                await EmailService.SendAsync("Zamówienie: " + orderId + " - zmieniono status na: '" + statusCaption + "'", string.Empty, companyInfo.Email, null, companyInfo);
            }
            #endregion send change satus email
        }
    }
}
