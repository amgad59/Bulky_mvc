using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Empire.Utilities
{
	public static class SD
	{
		public const string Role_Customer = "Customer";
		public const string Role_Employee = "Employee";
		public const string Role_Admin = "Admin";
		public const string Role_Company = "Company";

		public const string StatusPending = "Pending";
		public const string StatusApproved = "Approved";
		public const string StatusInProcess = "Processing";
		public const string StatusShipped = "Shipped";
		public const string StatusCancelled = "Cancelled";
		public const string StatusRefunded = "Refunded";

		public const string PaymentStatusPending = "Pending";
		public const string PaymentStatusApproved = "Approved";
		public const string PaymentStatusRejected = "Rejected";

		public const string SessionCart = "SessionShoppingCart";



        public static Dictionary<string, object> FirstPayload = new Dictionary<string, object> {
                    {
                    "auth_token", ""
                    },
                    {"delivery_needed", "false"},
                    { "amount_cents", "" },
                    { "currency", "EGP" },
                    { "items", ""}
            };
        public static Dictionary<string, object> SecondPayload = new Dictionary<string, object> {

                    { "auth_token","" },
                    { "amount_cents", "" },
                    { "expiration", 3600 },
                    { "order_id", "" },
                    { "billing_data",new Dictionary<string, object>{
                        {"apartment", "NA" },
                        {"email", "" },
                        {"floor", "NA"},
                        {"first_name", ""},
                        {"street", ""},
                        {"building", "NA"},
                        {"phone_number", ""},
                        {"shipping_method", "NA"},
                        {"postal_code", "NA"},
                        {"city", ""},
                        {"country", "NA"},
                        {"last_name", "Nicolas"},
                        { "state", "NA"} }
                    },
                    { "currency", "EGP"},
                    { "integration_id", 3951279},
                    { "lock_order_when_paid", "true"}
                };
        public enum ApiType
        {
            GET, POST, PUT, DELETE
        }
    }
}
