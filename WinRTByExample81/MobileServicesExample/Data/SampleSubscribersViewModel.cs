using System;
using System.Collections.Generic;
using System.Linq;

namespace MobileServicesExample
{
    public class SampleSubscribersViewModel : SubscribersViewModel
    {
        private readonly IList<Subscriber> _sampleSubscribers = new List<Subscriber>(new []
                                                                {
                                                                  new Subscriber
                                                                  {
                                                                      Id = Guid.NewGuid().ToString(),
                                                                      FirstName = "Rhonda",
                                                                      LastName = "Engelhardt",
                                                                      MailingAddress = "179 Winding Way, North Kingstown, RI 02852",
                                                                      Phone = "401-578-9622",
                                                                      Gender = Gender.Female,
                                                                      EmailAddress = "RhondaJEngelhardt@einrot.com"
                                                                  },
                                                                  new Subscriber
                                                                  {
                                                                      Id = Guid.NewGuid().ToString(),
                                                                      FirstName = "James",
                                                                      LastName = "Willis",
                                                                      MailingAddress = "4682 Poe Lane, Kansas City, KS 66207",
                                                                      Phone = "913-642-0938",
                                                                      Gender = Gender.Male,
                                                                      EmailAddress = "JamesJWillis@dayrep.com"
                                                                  }
                                                                });

        public SampleSubscribersViewModel()
        {
            Subscribers = _sampleSubscribers;
            SelectedSubscriber = Subscribers.FirstOrDefault();
        }
    }
}