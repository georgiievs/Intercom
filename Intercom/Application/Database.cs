using Intercom.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;

namespace Intercom.Application
{
    public class Database
    {
        public Database()
        {
            Apartments = Enumerable.Range(1, 85)
                .Select(code => new Apartment
                {
                    Code = code,
                    IsBlocked = false,
                });

            Keys = Enumerable.Range(1, 10)
                .Select(code => new Key
                {
                    Id = code,
                    ApartmentId = Random.Shared.Next(1, 80)
                });
            
        }

        public IEnumerable<Apartment> Apartments { get; private set; }

        public IEnumerable<Key> Keys { get; private set; }
    }
}
