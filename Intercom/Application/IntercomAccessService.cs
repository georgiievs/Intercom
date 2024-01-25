using Intercom.Data;
using System.Linq;

namespace Intercom.Application
{
    public class IntercomAccessService
    {
        private readonly IntercomDbContext database;

        public IntercomAccessService(IntercomController intercom)
        {
            database = intercom.Database;
        }

        public bool IsApartmentExist(int apartmentCode)
        {
            return database.Apartments.Any(a => a.Code == apartmentCode);
        }

        public bool IsKeyValid(int key)
        {
            return database.Keys.Any(k => k.Id == key);
        }

        public bool IsApartmentBlocked(int apartmentCode)
        {
            var apartment = database.Apartments
                .FirstOrDefault(x => x.Code == apartmentCode);

            return apartment == null || apartment.IsBlocked;
        }

        public bool BlockApartment(int apartmentCode)
        {
            var apartment = database.Apartments
                .FirstOrDefault(x => x.Code == apartmentCode);

            if(apartment == null)
            {
                return false;
            }

            apartment.IsBlocked = true;

            database.Update(apartment);
            database.SaveChanges();

            return true;
        }

        public bool UnblockApartment(int apartmentCode)
        {
            var apartment = database.Apartments
                    .FirstOrDefault(x => x.Code == apartmentCode);

            if(apartment == null)
            {
                return false;
            }

            apartment.IsBlocked = false;

            database.Update(apartment);
            database.SaveChanges();

            return true;
        }
    }
}
