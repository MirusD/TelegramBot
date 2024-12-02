using LinqToDB.Data;
using static _102techBot.Program;

namespace _102techBot.Infrastructure.DAL
{
    internal class DatabaseInitializer
    {
        private readonly AppDbContext _dbConnection;

        public DatabaseInitializer(AppDbContext dbContext)
        {
            _dbConnection = dbContext;
        }

        public async Task CreateTablesIfNotExists()
        {
            await CreateUsersTableIfNotExists();
            await CreateCategoriesTableIfNotExists();
            await CreateProductsTableIfNotExists();
            await CreateCartsTableIfNotExists();
            await CreateCartItemsTableIfNotExists();
            await CreateOrdersTableIfNotExists();
            await CreateOrderItemsTableIfNotExists();
            await CreateRepairsTableIfNotExists();
            await CreateCallbackTableIfNotExists();
            await CreateAddressTableIfNotExists();
        }

        private async Task CreateUsersTableIfNotExists()
        {
            var createTableSql = @"
            CREATE TABLE IF NOT EXISTS ""Users"" (
                ""Id"" BIGINT PRIMARY KEY,
                ""UserName"" VARCHAR(100),
                ""FirstName"" VARCHAR(100),
                ""LastName"" VARCHAR(100),
                ""Email"" VARCHAR(100),
                ""Phone"" VARCHAR(20),
                ""Role"" INT,
                ""State"" INT,
                ""CreatedAt"" TIMESTAMPTZ DEFAULT CURRENT_TIMESTAMP
            );
        ";
            await _dbConnection.ExecuteAsync(createTableSql);
        }

        private async Task CreateCategoriesTableIfNotExists()
        {
            var createTableSql = @"
            CREATE TABLE IF NOT EXISTS ""Categories"" (
                ""Id"" BIGSERIAL PRIMARY KEY,
                ""Name"" VARCHAR(255) NOT NULL,
                ""Description"" TEXT NULL,
                ""CreatedAt"" TIMESTAMPTZ DEFAULT CURRENT_TIMESTAMP
            );
        ";
            await _dbConnection.ExecuteAsync(createTableSql);
        }

        private async Task CreateProductsTableIfNotExists()
        {
            var createTableSql = @"
            CREATE TABLE IF NOT EXISTS ""Products"" (
                ""Id"" BIGSERIAL PRIMARY KEY,
                ""Name"" VARCHAR(255) NULL,
                ""Description"" TEXT NULL,
                ""Price"" DECIMAL(18, 2) NULL,
                ""Stock"" INT NULL,
                ""ImageUrl"" VARCHAR(255) NULL,
                ""CategoryId"" BIGINT NULL,
                ""UserId"" BIGINT,
                ""Status"" INT,
                ""CreatedAt"" TIMESTAMPTZ DEFAULT CURRENT_TIMESTAMP
            );
        ";
            await _dbConnection.ExecuteAsync(createTableSql);
        }

        private async Task CreateCartsTableIfNotExists()
        {
            var createTableSql = @"
            CREATE TABLE IF NOT EXISTS ""Carts"" (
                ""Id"" BIGSERIAL PRIMARY KEY,
                ""UserId"" BIGINT,
                ""CreatedAt"" TIMESTAMPTZ DEFAULT CURRENT_TIMESTAMP,
                CONSTRAINT ""fk_user"" FOREIGN KEY (""UserId"") REFERENCES ""Users""(""Id"")
            );
        ";
            await _dbConnection.ExecuteAsync(createTableSql);
        }

        private async Task CreateCartItemsTableIfNotExists()
        {
            var createTableSql = @"
            CREATE TABLE IF NOT EXISTS ""CartItems"" (
                ""Id"" BIGSERIAL PRIMARY KEY,
                ""CartId"" BIGINT NOT NULL,
                ""ProductId"" BIGINT NOT NULL,
                ""Quantity"" INT NOT NULL,
                ""CreatedAt"" TIMESTAMPTZ DEFAULT CURRENT_TIMESTAMP,
                CONSTRAINT ""fk_cart"" FOREIGN KEY (""CartId"") REFERENCES ""Carts""(""Id""),
                CONSTRAINT ""fk_product"" FOREIGN KEY (""ProductId"") REFERENCES ""Products""(""Id"")
            );
        ";
            await _dbConnection.ExecuteAsync(createTableSql);
        }

        private async Task CreateOrdersTableIfNotExists()
        {
            var createTableSql = @"
            CREATE TABLE IF NOT EXISTS ""Orders"" (
                ""Id"" BIGSERIAL PRIMARY KEY,
                ""UserId"" BIGINT,
                ""TotalAmount"" DECIMAL(18, 2),
                ""Status"" INT,
                ""CreatedAt"" TIMESTAMPTZ DEFAULT CURRENT_TIMESTAMP,
                CONSTRAINT ""fk_user"" FOREIGN KEY (""UserId"") REFERENCES ""Users""(""Id"")
            );
        ";
            await _dbConnection.ExecuteAsync(createTableSql);
        }

        private async Task CreateOrderItemsTableIfNotExists()
        {
            var createTableSql = @"
            CREATE TABLE IF NOT EXISTS ""OrderItems"" (
                ""Id"" BIGSERIAL PRIMARY KEY,
                ""OrderId"" BIGINT,
                ""ProductId"" BIGINT,
                ""Quantity"" INT NOT NULL,
                ""Price"" DECIMAL(18, 2),
                ""CreatedAt"" TIMESTAMPTZ DEFAULT CURRENT_TIMESTAMP,
                CONSTRAINT ""fk_order"" FOREIGN KEY (""OrderId"") REFERENCES ""Orders""(""Id""),
                CONSTRAINT ""fk_product"" FOREIGN KEY (""ProductId"") REFERENCES ""Products""(""Id"")
            );
        ";
            await _dbConnection.ExecuteAsync(createTableSql);
        }

        private async Task CreateRepairsTableIfNotExists()
        {
            var createTableSql = @"
            CREATE TABLE IF NOT EXISTS ""Repairs"" (
                ""Id"" BIGSERIAL PRIMARY KEY,
                ""UserId"" BIGINT NOT NULL,
                ""ProductId"" BIGINT NOT NULL,
                ""Description"" TEXT NOT NULL,
                ""Status"" INT NOT NULL,
                ""Phone"" TEXT NOT NULL,
                ""CategoryId"" INT NOT NULL,
                ""CreatedAt"" TIMESTAMPTZ DEFAULT CURRENT_TIMESTAMP,
                FOREIGN KEY (""UserId"") REFERENCES ""Users""(""Id""),
                FOREIGN KEY (""ProductId"") REFERENCES ""Products""(""Id""),
                FOREIGN KEY (""CategoryId"") REFERENCES ""Categories""(""Id"")
            );
        ";
            await _dbConnection.ExecuteAsync(createTableSql);
        }

        private async Task CreateCallbackTableIfNotExists()
        {
            var createTableSql = @"
            CREATE TABLE IF NOT EXISTS ""Callbacks"" (
                ""Id"" BIGSERIAL PRIMARY KEY,
                ""UserId"" BIGINT NOT NULL,
                ""Status"" INT NOT NULL,
                ""Phone"" TEXT NOT NULL,
                ""CreatedAt"" TIMESTAMPTZ DEFAULT CURRENT_TIMESTAMP,
                CONSTRAINT ""fk_user"" FOREIGN KEY (""UserId"") REFERENCES ""Users""(""Id"")
            );
        ";
            await _dbConnection.ExecuteAsync(createTableSql);
        }

        private async Task CreateAddressTableIfNotExists()
        {
            var createTableSql = @"
            CREATE TABLE IF NOT EXISTS ""Addresses"" (
                ""Id"" BIGSERIAL PRIMARY KEY,
                ""Street"" TEXT NOT NULL,
                ""City"" TEXT NOT NULL,
                ""PostalCode"" TEXT NOT NULL,
                ""Country"" TEXT NOT NULL,
                ""Description"" TEXT,
                ""Latitude"" DOUBLE PRECISION NOT NULL,
                ""Longitude"" DOUBLE PRECISION NOT NULL,
                ""WorkingHours"" TEXT NOT NULL,
                ""Phone"" TEXT NOT NULL,
                ""CreatedAt"" TIMESTAMPTZ DEFAULT CURRENT_TIMESTAMP
            );
        ";
            await _dbConnection.ExecuteAsync(createTableSql);
        }
    }
}
