
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace CollegaApp.Data.Repostory
{
    public class EntityRepostory<T> : IEntityRepostory<T> where T : class,IEntity
    {
        private readonly CollegeDBContext _dbContext;
        private readonly DbSet<T> _dbSet;
        public EntityRepostory(CollegeDBContext dbContext)
        {
            _dbContext = dbContext;
            _dbSet = _dbContext.Set<T>();//iste db deki tablomuzu temsil edecek yer burasi 
            //Burasi _dbContext.Student e  karsilik geliyor..
            //_dbContext.Set<Student>() ≈ _dbContext.Students mi? Evet. Eğer CollegeDBContext içinde:public DbSet<Student> Students { get; set; } tanımlıysa, iki çağrı da aynı DbSet’i verir:YANI _dbContext.Students = _dbContext.Set<Student>() Fark: İlki strongly-typed property ,İkincisi generic; generic repository içinde tabloyu tipten çıkararak alman gerektiğinde kullanırsın.(intellisense, naming).
            //DbSet<T> neyi temsil eder?
            //DbSet<T> = veritabanındaki T tablosunun tamamını ve o tabloya yönelik işlemleri temsil eden koleksiyon benzeri API’dir:Sorgu: Where/Select/FirstOrDefaultAsync (IQueryable → SQL’e çevrilir), Komut: Add/AddAsync, Update, Remove, Takip: Eklenen/silinecek/güncellenecek entity’lerin state’ini DbContext’e bildirir; asıl SQL’i SaveChangesAsync() üretir.Kısaca: DbSet<T> “tablo+işlemler” demektir.
        }


        public async Task<T> CreateAsync(T entity)//T dbRecord da diyebilirdik..
        {
            /*YONTEM-1*/
            //_dbSet=>_dbContext.Set<T>();=>_dbContext.Students....
            await _dbSet.AddAsync(entity);//unit of work-AddAsync çağrısı hemen DB’ye gitmez; state = Added yapılır.
            await _dbContext.SaveChangesAsync();//saved the db now...Asıl SQL, SaveChangesAsync() ile üretilir ve yürütülür (Unit of Work).
           // return entity.Id; //IEntity kısıtı sayesinde entity.Id güvenle dönebilirsin.HARIKA BESTPRACTISE.......Generic constraint olarak class,IEntity ekledik ve IEnity de Id barindiriyor artik!!!!!!!!Biz enityt deki id yi donmek istese idik de bu anlatgimz sekilde donebilirdik..ama biz entity i return etmek istiyoruz
             return entity;

        }

        public async Task<bool> DeleteAsync(T entityToDelete)
        {
            _dbSet.Remove(entityToDelete);
            await _dbContext.SaveChangesAsync();
            return true;
        }


        public async Task<List<T>> GetAllAsync()
        {
            return await _dbSet.AsNoTracking().ToListAsync();
        }


        //  public async Task<T?> GetByIdAsync(int id, bool isNoTracking = false)
        public async Task<T?> GetAsync(Expression<Func<T, bool>> filter, bool isNoTracking = false)
        {
            //if (isNoTracking) return await _dbSet.AsNoTracking().FirstOrDefaultAsync(s => s.Id == id) ?? null;
            //return await _dbSet.FirstOrDefaultAsync(s => s.Id == id) ?? null;
            if (isNoTracking) return await _dbSet.AsNoTracking()
                    .FirstOrDefaultAsync(filter) ?? null;
            //Dikkat edelim biz delegate tanimini gelip de FirstOrDefaultAsync icine veremeyiz..delegate ile olsuturulmus degisken i verebliriz...ki filter da nedir biliyormuyuz bu iste aslinda (T)=>return bool...bunu temsil ediyor..paramtreye entityi ver o entity uzerinden karsilastirma yapabilirsin sonucu bool donedcek sekilde diyor...HARIKA BESTPRACTISE..AYNI JAVASCRIPTTE, TYPESCRIPT ILE BERABER KULLANIP DA PARAMTREYE ARROW FUNC VERECEGIMZ ZAMAN BOOL DONEN, PARAMTREDE ONUN GIRIS TIPINI,VS VERIRIRZ..BIRDE EXPRESSOIN ILE VERMELIYZ BURDA CUNKU EXPRESSION BUNU DATA OLARAK EF-FRAMEWORKE VERIYOR VE BU SQL ICINDE KULLANILACAK, ORDA KULLANILACAGI ICIN AMA DIREK OLARAK RAM DE KULLANILSA IDI, EXPRESSION A GEREK KALLMAYACAKTI...
            return await _dbSet.FirstOrDefaultAsync(filter) ?? null;
            //Simdi biz burda spesifik olarak bir Student,Course,Teacher entity ye ait bir propertyi kullanmamamiz gerekiyor ve burdaki lambda lari da genellestirmemiz ve oraya bizim predicate, yani delegate kullanmamiz gerekiyor...
        }
        public async Task<T> UpdateAsync(T entity)
        {
            _dbSet.Update(entity);
            await _dbContext.SaveChangesAsync();
            return entity;
        }
    }
}

/*
 public class EntityRepository<TEntity, TContext> : IEntityRepository<TEntity>
    where TEntity  : class, IEntity
    where TContext : DbContext, new()

Generic Consraints

  where TEntity  : class, IEntity
    where TContext : DbContext, new()


Derleyiciye “TEntity ve TContext şu özelliklere sahip olmalı” diyorsun ki içeride güvenle belirli işlemleri yapabilesin.
where TEntity  : class, IEntity
where TContext : DbContext, new()

Ne anlama geliyor?
where TEntity : class, IEntity
class: TEntity bir referans tip olmalı (struct olamaz). EF Core DbSet<T> referans tiplerle çalışır.
IEntity: TEntity mutlaka IEntity arayüzünü implemente etmeli.
Böylece içeride entity.Id gibi üyeleri derleme zamanında kullanabilirsin (çünkü IEntity onu garanti eder).

public async Task<int> CreateAsync(T entity)
        {
            await _dbContext.Set<T>().AddAsync(entity);//unit of work
            await _dbContext.SaveChangesAsync();//saved the db now...
            return entity.Id;//HARIKA BESTPRACTISE..NORMALDE .Id yapinca bulamiyordu bu Enity de Id propertysinin oldugunu ve hata veriyordu, ne zaman ki yukarda Generic constraint olarak class,IEntity ekledik ve IEnity de Id barindiriyor o zaman iste bu hata ortadan kalkti

Sonuç: Set<TEntity>(), AddAsync(entity), entity.Id gibi kodlar güvenli ve IntelliSense’li.

where TContext : DbContext, new()
DbContext: TContext bir EF Core DbContext olmalı.
Böylece new TContext() ile oluşturduğun nesnede Set<TEntity>(), SaveChangesAsync() gibi üyelerin varlığını derleyici garanti eder (ve IDisposable olduğu da garantidir).
new(): parametresiz public ctor olmalı ki new TContext() diyebilesin.
Sonuç: using var context = new TContext(); context.Set<TEntity>() ... yazabilirsin.

DELEGATE-FUNC DELEGATE-PREDICATE KULLANIMI...


   //   public async Task<T?> GetByEmailAsync(string email)
        public async Task<T?> GetByEmailAsync(Expression<Func<T,bool>> filter)
        {
            //Burda da normalde s.Email de hata verdi bulamadi, ama biz IEntiy yi Student e implement e ettrdik ve I Entity ye Id nin yaninda Email ve Name i e ekledik sonra hata ortadan kalkti
            //Simdi biz burda spesifik olarak bir Student,Course,Teacher entity ye ait bir propertyi kullanmamamiz gerekiyor ve burdaki lambda lari da genellestirmemiz ve oraya bizim predicate, yani delegate kullanmamiz gerekiyor...
            // return await _dbSet.AsNoTracking().FirstOrDefaultAsync(s => s.Email == email);
            return await _dbSet.AsNoTracking().FirstOrDefaultAsync(filter);
            //DIKKAT DELEGATE DEGSKENINI VERDIK YANI, PREDICATE YANI FUN<T,BOOL>> BOOL DONEN, ARROW FUNC I TUTAN DEGSKENI EXPRESSINO ILE ATADIK...CUNKU IQUERABLE DA SQL QUERY DE KULLANILACAGI ICIN, EXPRESSION ILE DATA OLARAK TUTYOR BU PREDICATE I KI SQL QUERY DE KULLANILIRKEN ANLAYABILSIN

            //Expression<Func<T,bool>> = “veri” (ifadenin ağacı). EF Core bunu okur ve SQL’e çevirir.
           // Func<T, bool>(predicate / delegate) = “çalıştırılabilir kod”. EF Core bunu SQL’e çeviremez; ancak bellekte çalıştırılır.

Func<T,bool> (predicate/delegate) bu aslinda sunu mu temsil ediyor (T)=>bool..yan bildigmz return edilmis veya call-invoke edilmis bir fonksiyonu mu temsil ediyor da ondan biz onu ef-core un okuyup sql e cevirmesine imkan kalmiyor direk calisir halde oldugu icin bunu mu diyorsun?

Evet—tam olarak mesele bu.

Func<T, bool> (predicate/delegate)
Bu, çalıştırılabilir bir fonksiyon tipidir. Lambda’yı bu tipe atadığında derleyici onu IL koduna derler (delegate). Artık elinde sadece “çağrılabilir kod” vardır, ifadenin yapısı (ağacı) yoktur. EF Core bu kodun içini okuyup “bunu SQL’e çevireyim” diyemez; sadece RAM’de çalıştırabilir (LINQ to Objects).
Örnek:

Func<Student, bool> pred = s => s.Name.StartsWith(prefix);
// pred burada derlenmiş bir fonksiyon (invoke edilebilir), expression değil.

Expression<Func<T, bool>>
Bu ise ifade ağacıdır (expression tree): s => ...’in temsilidir (veri olarak AST). EF Core bu ağacı parçalar, analiz eder ve SQL üretir. Yani “çalıştırmaya hazır kod” değil, “kodun yapısal temsili” elindedir.
Örnek:

Expression<Func<Student, bool>> expr = s => s.Name.StartsWith(prefix);
// expr burada 'StartsWith' çağrısı, 'Name' üyeye erişim, 'prefix' sabiti vb. düğümlerden oluşan bir ağaç.

Neden EF Core Func<T,bool>’ü çeviremez?
Delegate = derlenmiş, yürütülebilir kod (siyah kutu).
EF’nin ihtiyacı = temsil (hangi üye, hangi çağrı, hangi sabit?) ⇒ bunu sadece expression tree verir.

Lambda neden bazen delegate, bazen expression oluyor?
Aynı lambda, hedef tipe göre derlenir:

// delegate
Func<Student, bool> p1 = s => s.Id == 5;

// expression tree
Expression<Func<Student, bool>> p2 = s => s.Id == 5;

FirstOrDefaultAsync gibi EF uzantıları Expression<Func<...>> bekler; bu yüzden
await _dbSet.FirstOrDefaultAsync(s => s.Id == 5);

satırında lambda expression tree olarak yakalanır ve SQL’e çevrilir.

Pratik kural

Veritabanında filtrelemek istiyorsan → Expression<Func<T,bool>> (server-side).

Önce listeyi çekip RAM’de filtreleyeceksen → Func<T,bool> (client-side).

Kısacası: Func<T,bool> = (T) => bool şeklinde çağrılabilir fonksiyon; EF bunu çeviremez.
Expression<Func<T,bool>> = (T) => bool’un veri olarak temsili; EF bunu SQL’e çevirebilir.

Neden Expression gerekli?

Çünkü EF Core çalıştırılabilir kodu (delegate) değil, ifadenin yapısını (ağaç) görürse SQL üretebilir.

Func<T,bool> verdiğinde EF “bunu nasıl SQL yapayım?” diyemez; EF Core 3.0+’da client eval büyük ölçüde kapandığı için ya exception alırsın ya da (AsEnumerable/ToList yaptıysan) RAM’de çalışır.

Ne zaman Expression<Func<T,bool>> kullanırım?
Veritabanına gidecek sorgularda (yani IQueryable<T> üstünde):
EF ifadenin expression tree’sini parse eder → SQL üretir → server-side filtreler.

public Task<T?> GetOneAsync(Expression<Func<T,bool>> filter, bool noTracking = false)
{
    IQueryable<T> q = _dbSet;
    if (noTracking) q = q.AsNoTracking();
    return q.FirstOrDefaultAsync(filter); // SQL'e çevrilir
}
Bu, en yaygın ve doğru imzadır; repository içinde genel filtreyi böyle alırsın.


Ne zaman sadece Func<T,bool> (predicate) kullanırım?

Belleğe aldıktan sonra (LINQ to Objects) filtreleme yapacaksam:

ToListAsync()/AsEnumerable() sonrasında

EF’in çeviremeyeceği özel bir .NET metodu kullanacaksam

// 1) DB'den çek (server-side filtre yok)
var list = await _dbSet.AsNoTracking().ToListAsync();

// 2) RAM'de, LINQ to Objects ile filtrele (Func<T,bool>)
var result = list.FirstOrDefault(x => SomeDotNetOnlyMethod(x.Name));

Not: Büyük veride bu yaklaşım pahalıdır (önce hepsini çekersin). O yüzden mümkün oldukça Expression ile server-side filtrelemeyi tercih et.

Özet “Best Practice”

Repository sorgu imzaları Expression<Func<T,bool>> alsın → server-side çalışsın.

RAM’de çalıştırman gerekiyorsa (özel .NET metodu, küçük veri), o zaman Func<T,bool> kullan; ama bunun DB tarafında filtrelemeyeceğini bil.

FirstOrDefaultAsync, AnyAsync, CountAsync gibi EF uzantıları IQueryable + Expression ile çalışır.
IEnumerable’a düştüğünde (ToList/AsEnumerable sonra) senkron LINQ to Objects çağrılarını (FirstOrDefault, Any) kullanırsın.


Kısa hatırlatma örnekleri

Server-side (doğru, expression):

await _dbSet.FirstOrDefaultAsync(x => x.Email == email);
await _dbSet.Where(x => x.Name.StartsWith(prefix)).ToListAsync();


Client-side (bellekte, predicate): TABI BURDA CLIENT DEDIGI DE YINE BIZIM RAM YANI SERVER IMIZ DAN DB-SERVER DAN DATA CEKILDIGI ICIN BU ILIKSKIDE SERVER-CLIENT SIDE, DB-SERVER=>SERVER SIDE OLUYOR

var data = await _dbSet.ToListAsync();                  // hepsini çek
var found = data.FirstOrDefault(x => MyNetOnly(x.Name)); // .NET kodu RAM'de

******************************* “ne derlenir / ne derlenmez” ****************************

1) Doğru imza

// EF Core uzantıları böyle bir imza bekler:
Task<T?> FirstOrDefaultAsync(Expression<Func<T,bool>> predicate);

Gerekli using’ler:

using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;


2) Doğrudan lambda VER (derleyici Expression’a çevirir)

var item = await _dbSet               // IQueryable<T>
    .AsNoTracking()
    .FirstOrDefaultAsync(s => s.Id == id);  // ✔ DERLENİR (lambda -> Expression<Func<T,bool>>)

Burada tip yazmıyorsun. Sadece s => s.Id == id lambda’sını veriyorsun.
Metot Expression<Func<T,bool>> beklediği için derleyici lambdayı expression tree olarak kurar.

3) İstersen önce değişkene ATA, sonra ver (ikisi aynı)

Expression<Func<T,bool>> filter = s => s.Id == id; // ✔ DERLENİR
var item = await _dbSet.AsNoTracking().FirstOrDefaultAsync(filter); // ✔

Burada filter zaten Expression<Func<T,bool>>.
Yine aynı şey; sadece adı var.


4) ŞU YANLIŞ (tip adı argüman yerine yazılamaz)
// ✘ BU SÖZDİZİMİ YANLIŞ:
await _dbSet.AsNoTracking().FirstOrDefaultAsync(Expression<Func<T,bool>>);

Burada tip ismi yazmaya çalışıyorsun. Argüman bekleyen yere değer (lambda/değişken) verilir, tip verilmez.
Doğrusu: ya doğrudan lambda ver, ya da önceden bir Expression<Func<T,bool>> değişkeni oluşturup onu ver (2 ve 3. örnek).

5) Peki Func<T,bool> geçirirsem?

Func<T,bool> pred = s => s.Id == id;

// ✘ Bu DERLENMEZ (EF metodu Expression bekliyor):
await _dbSet.AsNoTracking().FirstOrDefaultAsync(pred);

Metot Expression<Func<T,bool>> bekliyor ama sen Func<T,bool> verdin → imza uymuyor.
Çözüm: ya argümanı Expression<...> yap, ya da önce veriyi RAM’e alıp LINQ to Objects’te kullan:

var list = await _dbSet.AsNoTracking().ToListAsync();
var item = list.FirstOrDefault(pred); // ✔ burada Func çalışır (RAM’de)
Not: Büyük tabloda bu pahalıdır; mümkün olduğunda Expression kullanıp filtreyi SQL tarafında yap.

6) Sık yapılan iki hata ve çözüm
System.Linq.Expressions using’i yok → Expression<...> tipi görünmez.
➜ using System.Linq.Expressions; ekle.

_dbSet aslında IEnumerable<T> (ör. daha önce ToList() çağırmışsın)
➜ FirstOrDefaultAsync değil, senkron FirstOrDefault çağrılır; Expression işe yaramaz.
_dbSet her zaman IQueryable<T> (EF query) olarak kalsın; ToList’i en sonda yap.

Kısa özet

Doğrudan lambda ver: FirstOrDefaultAsync(s => s.Id == id) ✔

İstersen önce değişkene ata: Expression<Func<T,bool>> filter = ...; FirstOrDefaultAsync(filter) ✔

Tip adı argüman yerine yazılamaz: FirstOrDefaultAsync(Expression<Func<T,bool>>) ✘

Func veremezsin (server-side için): FirstOrDefaultAsync(pred) ✘ → ya Expression yap ya da RAM’de çalış.

*********************************Expression sadece FirstOrDefaultAsync’te çalışmıyor.*****************

Kural şu: Expression’lar, IQueryable<T> üstünde çalışan tüm LINQ operatörlerinde (sync veya async) SQL’e çevrilir. ToList/AsEnumerable ile listeye çevirmeden önce olduğun sürece çalışır.

Ne zaman Expression çalışır?

IQueryable<T> üzerinde ve operatör Expression bekliyorsa:

Where(x => …), Select(x => …), OrderBy(x => …), Any(x => …), Count(x => …), FirstOrDefault(x => …) … ✔

Async karşılıkları: ToListAsync(), FirstOrDefaultAsync(...), AnyAsync(...), CountAsync(...) … ✔

Sync/Async farkı sadece çalıştırma şekli; çeviri mantığı aynı:

// SYNC (IQueryable, Expression → SQL)
var s1 = _dbSet.FirstOrDefault(x => x.Id == id);        // ✔ SQL

// ASYNC (IQueryable, Expression → SQL)
var s2 = await _dbSet.FirstOrDefaultAsync(x => x.Id == id); // ✔ SQL

Ne zaman Expression çalışmaz?

Materialize ettikten sonra (veriyi RAM’e aldıktan sonra):
var list = await _dbSet.ToListAsync();   // ↓ buradan sonrası IEnumerable/Objects
var s3 = list.FirstOrDefault(x => x.Id == id); // Func çalışır, SQL’e çevrilmez ❌

Burada artık IEnumerable<T>’desin; operatörler Func<T,...> ister ve RAM’de çalışır.
Hızlı rehber

IQueryable<T> + Where/Select/FirstOrDefault/Any/Count (sync) → Expression → SQL ✔

IQueryable<T> + ...Async (async) → Expression → SQL ✔

IEnumerable<T> (ToList/AsEnumerable sonrası) → Func → RAM ❌ (SQL çevirisi yok)

ASP.NET Core’da async yöntemleri tercih et (örn. FirstOrDefaultAsync, ToListAsync), ama unutma: sync muadilleri de IQueryable üstündeyse Expression’ı SQL’e çevirir; sadece thread’i bloklar.

 */