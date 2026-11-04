using Google.Cloud.Firestore;
using Google.Cloud.Storage.V1;
using Microsoft.Extensions.DependencyInjection;
using SameApi.Db.DbContexts;
using SameApi.Db.Repository;
using SameApi.Db.Repository.Implementation;
using SameApi.Db.UnitOfWork;

namespace SameApi.Db
{
    public static class DbRegisterExtension
    {
        public static void RegisterSameApiDbContainer(this IServiceCollection services)
        {

            services.AddScoped<IApiSameDbContext, SameApiDbContext>();

            services.AddScoped<IUserRepository, UserRepository>();
            services.AddScoped<IPostRepository, PostRepository>();
            services.AddScoped<ICommentRepository, CommentRepository>();

            services.AddScoped<IApiSameUnitOfWork, SameApiUnitOfWork>();

            services.AddSingleton(StorageClient.Create());
        }

        public static void RegisterFireStore(this IServiceCollection services, string projectId, string databaseId)
        {
            services.AddSingleton(_ => new FirestoreDbBuilder
            {
                ProjectId = projectId,
                DatabaseId = databaseId
            }.Build());
        }

        #region seed
        public static async Task SeedGendersAsync(FirestoreDb db)
        {
            var batch = db.StartBatch();
            foreach (var (id, data) in Genders)
                batch.Set(db.Collection("genders").Document(id), data);
            await batch.CommitAsync();
        }

        public static async Task SeedSchoolsAsync(FirestoreDb db)
        {
            var batch = db.StartBatch();
            foreach (var (id, data) in Schools)
                batch.Set(db.Collection("schools").Document(id), data);
            await batch.CommitAsync();
        }
        private static readonly Dictionary<string, Dictionary<string, object>> Genders = new()
        {
            ["male"] = new() { ["label"] = "Homme", ["order"] = 1 },
            ["female"] = new() { ["label"] = "Femme", ["order"] = 2 },
            ["other"] = new() { ["label"] = "Autre", ["order"] = 3 },
        };

        private static readonly Dictionary<string, Dictionary<string, object>> Schools = new()
        {
            ["co-aubepine"] = new() { ["label"] = "CO de l'Aubépine", ["type"] = "co" },
            ["co-bois-caran"] = new() { ["label"] = "CO du Bois-Caran", ["type"] = "co" },
            ["co-bude"] = new() { ["label"] = "CO de Budé", ["type"] = "co" },
            ["co-cayla"] = new() { ["label"] = "CO du Cayla", ["type"] = "co" },
            ["co-colombieres"] = new() { ["label"] = "CO des Colombières", ["type"] = "co" },
            ["co-drize"] = new() { ["label"] = "CO de la Drize", ["type"] = "co" },
            ["co-florence"] = new() { ["label"] = "CO de Florence", ["type"] = "co" },
            ["co-foretaille"] = new() { ["label"] = "CO de Foretaille", ["type"] = "co" },
            ["co-golette"] = new() { ["label"] = "CO de la Golette", ["type"] = "co" },
            ["co-grandes-communes"] = new() { ["label"] = "CO des Grandes-Communes", ["type"] = "co" },
            ["co-livron"] = new() { ["label"] = "CO de Livron", ["type"] = "co" },
            ["co-marais"] = new() { ["label"] = "CO du Marais", ["type"] = "co" },
            ["co-montbrillant"] = new() { ["label"] = "CO de Montbrillant", ["type"] = "co" },
            ["co-orangerie"] = new() { ["label"] = "CO de l'Orangerie", ["type"] = "co" },
            ["co-pinchat"] = new() { ["label"] = "CO de Pinchat", ["type"] = "co" },
            ["co-renard"] = new() { ["label"] = "CO du Renard", ["type"] = "co" },
            ["co-secheron"] = new() { ["label"] = "CO de Sécheron", ["type"] = "co" },
            ["co-seymaz"] = new() { ["label"] = "CO de la Seymaz", ["type"] = "co" },
            ["co-voirets"] = new() { ["label"] = "CO des Voirets", ["type"] = "co" },

            ["col-calvin"] = new() { ["label"] = "Collège Calvin", ["type"] = "college" },
            ["col-candolle"] = new() { ["label"] = "Collège de Candolle", ["type"] = "college" },
            ["col-claparede"] = new() { ["label"] = "Collège Claparède", ["type"] = "college" },
            ["col-rousseau"] = new() { ["label"] = "Collège Rousseau", ["type"] = "college" },
            ["col-voltaire"] = new() { ["label"] = "Collège Voltaire", ["type"] = "college" },
            ["col-saussure"] = new() { ["label"] = "Collège de Saussure", ["type"] = "college" },
            ["col-sismondi"] = new() { ["label"] = "Collège Sismondi", ["type"] = "college" },
            ["col-stael"] = new() { ["label"] = "Collège de Staël", ["type"] = "college" },
            ["col-emilie-gourd"] = new() { ["label"] = "Collège Émilie-Gourd", ["type"] = "college" },
            ["col-nicolas-bouvier"] = new() { ["label"] = "Collège Nicolas-Bouvier", ["type"] = "college" },
            ["col-andre-chavanne"] = new() { ["label"] = "Collège André-Chavanne", ["type"] = "college" },

            ["ecg-ella-maillart"] = new() { ["label"] = "ECG Ella-Maillart", ["type"] = "ecg" },
            ["ecg-jean-piaget"] = new() { ["label"] = "ECG Jean-Piaget", ["type"] = "ecg" },
            ["ecg-henri-dunant"] = new() { ["label"] = "ECG Henri-Dunant", ["type"] = "ecg" },
            ["ec-aimee-stitelmann"] = new() { ["label"] = "École de commerce Aimée-Stitelmann", ["type"] = "commerce" },
            ["ec-raymond-uldry"] = new() { ["label"] = "École de commerce Raymond-Uldry", ["type"] = "commerce" },

            ["cfp-cfpt"] = new() { ["label"] = "CFPT (technique)", ["type"] = "cfp" },
            ["cfp-cfpc"] = new() { ["label"] = "CFPC (construction)", ["type"] = "cfp" },
            ["cfp-cfpaa"] = new() { ["label"] = "CFPAA (arts appliqués)", ["type"] = "cfp" },
            ["cfp-cfps"] = new() { ["label"] = "CFPS (santé et social)", ["type"] = "cfp" },
            ["cfp-cfpshr"] = new() { ["label"] = "CFPSHR (services, hôtellerie, restauration)", ["type"] = "cfp" },
        };
        #endregion
    }
}