using Customer.API.Models;

namespace Customer.API.Services;

public class CustomerRepository
{
    private readonly List<Models.Customer> _customers;

    public CustomerRepository()
    {
        _customers = GenerateCustomers();
    }

    public IEnumerable<Models.Customer> GetAll() => _customers;

    public Models.Customer? GetById(int id) => _customers.FirstOrDefault(c => c.Id == id);

    public IEnumerable<Models.Customer> SearchByName(string companyName) =>
        _customers.Where(c => c.Name.Contains(companyName, StringComparison.OrdinalIgnoreCase));

    public void Add(Models.Customer customer)
    {
        customer.Id = _customers.Any() ? _customers.Max(c => c.Id) + 1 : 1;
        _customers.Add(customer);
    }

    public bool Update(Models.Customer customer)
    {
        var index = _customers.FindIndex(c => c.Id == customer.Id);
        if (index == -1) return false;

        _customers[index] = customer;
        return true;
    }

    public bool Delete(int id)
    {
        var customer = _customers.FirstOrDefault(c => c.Id == id);
        if (customer == null) return false;

        _customers.Remove(customer);
        return true;
    }

    private List<Models.Customer> GenerateCustomers()
    {
        var customers = new List<Models.Customer>
        {
            new Models.Customer { Id = 1, Name = "Nexora Dynamics", Address = "4025 Springwood Ave", City = "Dallas", State = "TX", Zip = "75204", Email = "info@nexoradynamics.com" },
            new Models.Customer { Id = 2, Name = "Veltrix Systems", Address = "1208 N Pine St", City = "Phoenix", State = "AZ", Zip = "85006", Email = "contact@veltrixsystems.com" },
            new Models.Customer { Id = 3, Name = "Quantive Labs", Address = "3155 W Armitage Ave", City = "Chicago", State = "IL", Zip = "60647", Email = "hello@quantivelabs.com" },
            new Models.Customer { Id = 4, Name = "Zentrovia Tech", Address = "880 Union St", City = "Brooklyn", State = "NY", Zip = "11215", Email = "support@zentroviatech.com" },
            new Models.Customer { Id = 5, Name = "Kintara Solutions", Address = "1312 Capitol Ave", City = "Sacramento", State = "CA", Zip = "95814", Email = "info@kintarasolutions.com" },
            new Models.Customer { Id = 6, Name = "Brevora Analytics", Address = "7455 SW Barbur Blvd", City = "Portland", State = "OR", Zip = "97219", Email = "contact@brevoraanalytics.com" },
            new Models.Customer { Id = 7, Name = "Synergos Cloud", Address = "590 Tremont St", City = "Boston", State = "MA", Zip = "02118", Email = "hello@synergoscloud.com" },
            new Models.Customer { Id = 8, Name = "OptraLink Networks", Address = "1161 S Main St", City = "Salt Lake City", State = "UT", Zip = "84101", Email = "support@optralinknetworks.com" },
            new Models.Customer { Id = 9, Name = "Luminate Fusion", Address = "3900 W 11th Ave", City = "Denver", State = "CO", Zip = "80204", Email = "info@luminatefusion.com" },
            new Models.Customer { Id = 10, Name = "Trovexa Security", Address = "217 E Central Blvd", City = "Orlando", State = "FL", Zip = "32801", Email = "contact@trovexasecurity.com" },
            new Models.Customer { Id = 11, Name = "Norvant BioTech", Address = "2132 E 10th St", City = "Indianapolis", State = "IN", Zip = "46201", Email = "hello@norvantbiotech.com" },
            new Models.Customer { Id = 12, Name = "Altronic Innovations", Address = "1417 NE 63rd St", City = "Seattle", State = "WA", Zip = "98115", Email = "support@altronicinnovations.com" },
            new Models.Customer { Id = 13, Name = "Fenrock Digital", Address = "4823 S Harvard Ave", City = "Tulsa", State = "OK", Zip = "74135", Email = "info@fenrockdigital.com" },
            new Models.Customer { Id = 14, Name = "Stratison Robotics", Address = "2001 Market St", City = "Philadelphia", State = "PA", Zip = "19103", Email = "contact@stratisonrobotics.com" },
            new Models.Customer { Id = 15, Name = "Evolvia Enterprises", Address = "6754 N 1st Ave", City = "Tucson", State = "AZ", Zip = "85718", Email = "hello@evolviaenterprises.com" },
            new Models.Customer { Id = 16, Name = "Hexalight Media", Address = "1723 Lombard St", City = "San Francisco", State = "CA", Zip = "94123", Email = "support@hexalightmedia.com" },
            new Models.Customer { Id = 17, Name = "Trinora Ventures", Address = "902 Broad Ripple Ave", City = "Indianapolis", State = "IN", Zip = "46220", Email = "info@trinoraventures.com" },
            new Models.Customer { Id = 18, Name = "Solvanta AI", Address = "4010 E 8th Ave", City = "Tampa", State = "FL", Zip = "33605", Email = "contact@solvantaai.com" },
            new Models.Customer { Id = 19, Name = "Novexium Logistics", Address = "809 W Main St", City = "Boise", State = "ID", Zip = "83702", Email = "hello@novexiumlogistics.com" },
            new Models.Customer { Id = 20, Name = "Cybranet Industries", Address = "1331 G St NW", City = "Washington", State = "DC", Zip = "20005", Email = "support@cybranetindustries.com" }
        };

        return customers;
    }
}