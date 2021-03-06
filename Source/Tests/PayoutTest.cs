using Microsoft.VisualStudio.TestTools.UnitTesting;
using PayPal.Api;
using System.Collections.Generic;

namespace PayPal.Testing
{
    [TestClass()]
    public class PayoutTest
    {
        public static readonly string PayoutJson = 
            "{\"sender_batch_header\":" + PayoutSenderBatchHeaderTest.PayoutSenderBatchHeaderJson + "," +
            "\"items\":[" + PayoutItemTest.PayoutItemJson + "]}";

        public static Payout GetPayout()
        {
            return JsonFormatter.ConvertFromJson<Payout>(PayoutJson);
        }

        [TestMethod, TestCategory("Unit")]
        public void PayoutObjectTest()
        {
            var testObject = GetPayout();
            Assert.IsNotNull(testObject);
            Assert.IsNotNull(testObject.sender_batch_header);
            Assert.IsNotNull(testObject.items);
            Assert.IsTrue(testObject.items.Count == 1);
        }

        [TestMethod, TestCategory("Unit")]
        public void PayoutConvertToJsonTest()
        {
            Assert.IsFalse(GetPayout().ConvertToJson().Length == 0);
        }

        [TestMethod, TestCategory("Unit")]
        public void PayoutToStringTest()
        {
            Assert.IsFalse(GetPayout().ToString().Length == 0);
        }

        [TestMethod, TestCategory("Functional")]
        public void PayoutCreateTest()
        {
            var payout = PayoutTest.GetPayout();
            var payoutSenderBatchId = "batch_" + System.Guid.NewGuid().ToString().Substring(0, 8);
            payout.sender_batch_header.sender_batch_id = payoutSenderBatchId;
            var createdPayout = payout.Create(TestingUtil.GetApiContext(), false);
            Assert.IsNotNull(createdPayout);
            Assert.IsTrue(!string.IsNullOrEmpty(createdPayout.batch_header.payout_batch_id));
            Assert.AreEqual(payoutSenderBatchId, createdPayout.batch_header.sender_batch_header.sender_batch_id);
        }

        [TestMethod, TestCategory("Functional")]
        public void PayoutGetTest()
        {
            var payoutBatchId = "8NX77PFLN255E";
            var payout = Payout.Get(TestingUtil.GetApiContext(), payoutBatchId);
            Assert.IsNotNull(payout);
            Assert.AreEqual(payoutBatchId, payout.batch_header.payout_batch_id);
        }

        [Ignore]
        public static PayoutBatch CreateSingleSynchronousPayoutBatch()
        {
            return Payout.Create(TestingUtil.GetApiContext(), new Payout
            {
                sender_batch_header = new PayoutSenderBatchHeader
                {
                    sender_batch_id = "batch_" + System.Guid.NewGuid().ToString().Substring(0, 8),
                    email_subject = "You have a Payout!"
                },
                items = new List<PayoutItem>
                {
                    new PayoutItem
                    {
                        recipient_type = PayoutRecipientType.EMAIL,
                        amount = new Currency
                        {
                            value = "1.0",
                            currency = "USD"
                        },
                        note = "Thanks for the payment!",
                        sender_item_id = "2014031400023",
                        receiver = "shirt-supplier-one@gmail.com"
                    }
                }
            },
            true);
        }
    }
}
