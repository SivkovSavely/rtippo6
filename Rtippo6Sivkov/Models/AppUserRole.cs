using System.Collections;

namespace Rtippo6Sivkov.Models;

public class AppUserRole
{
    public int Id { get; set; }
    public string Name { get; set; } = null!;

    public List<AppUser> UsersWithRole { get; set; } = null!;
}

public static class AppUserExtensions
{
    /// <summary>
    /// Проверка прав пользователей. Имеет ли право данная роль искать и просматривать записи реестра организаций
    /// в данном муниципальном образовании?
    /// </summary>
    /// <param name="user">Пользователь, у которого проверяют права.</param>
    /// <param name="locality">Муниципальное образование, доступ к которому нужно проверить у данной роли.</param>
    /// <returns>true, если имеет право; false, если не имеет.</returns>
    public static bool CanReadRegistryEntry(this AppUser user, Organization organization)
    {
        return user.Role.Name.ToLower() switch
        {
            // Доступ для поиска и просмотра в рамках всех записей реестра
            "куратор ветслужбы"
                or "куратор по отлову"
                or "куратор приюта"
                or "подписант ветслужбы"
                or "подписант по отлову"
                or "подписант приюта" => true,

            // Доступ для поиска и просмотра в рамках записей, относящихся к соответствующему муниципальному образованию
            "куратор омсу"
                or "подписант омсу" => user.Locality.Id == organization.Locality.Id,

            // Доступ для ведения в части типов:
            // - Исполнительный орган государственной власти;
            // - Орган местного самоуправления;
            // - Ветеринарная клиника: государственная
            "оператор ветслужбы" => organization.Type.Name.ToLower() is
                "исполнительный орган государственной власти"
                or "орган местного самоуправления"
                or "ветеринарная клиника: государственная",

            // Доступ для ведения в рамках записей, относящихся к соответствующему муниципальному образованию, в составе следующих типов:
            // - Приют;
            // - Организация по отлову;
            // - Организация по отлову и приют;
            // - Организация по транспортировке;
            // - Ветеринарная клиника: государственная;
            // - Ветеринарная клиника: частная;
            // - Благотворительный фонд;
            // - Организации по продаже товаров и предоставлению услуг для животных
            "оператор омсу" => user.Locality.Id == organization.Locality.Id && organization.Type.Name.ToLower() is
                "приют"
                or "организация по отлову"
                or "организация по отлову и приют"
                or "организация по транспортировке"
                or "ветеринарная клиника: государственная"
                or "ветеринарная клиника: частная"
                or "благотворительный фонд"
                or "организации по продаже товаров и предоставлению услуг для животных",
            
            "гость" => false,
            
            "администратор" => true,
            
            _ => throw new Exception($"Неизвестная роль: {user.Role.Name}")
        };
    }

    /// <summary>
    /// Проверка прав пользователей. Имеет ли право данная роль изменять и добавлять записи реестра организаций
    /// в данном муниципальном образовании?
    /// </summary>
    /// <param name="user">Пользователь, у которого проверяют права.</param>
    /// <param name="organization">
    /// Муниципальное образование, доступ к которому нужно проверить у данной роли.
    /// Если locality == null, то муниципальное образование не берется в расчет проверки прав пользователя.
    /// Например, при попытке добавления записи (открытия формы) нет возможности узнать, в какое муниципальное
    /// образование будет попытка добавить запись.
    /// </param>
    /// <returns>true, если имеет право; false, если не имеет.</returns>
    public static bool CanCreateOrEditRegistryEntry(this AppUser user, Organization? organization = null)
    {
        return user.Role.Name.ToLower() switch
        {
            // Доступ для поиска и просмотра в рамках всех записей реестра
            "куратор ветслужбы"
                or "куратор по отлову"
                or "куратор приюта"
                or "подписант ветслужбы"
                or "подписант по отлову"
                or "подписант приюта" => false,

            // Доступ для поиска и просмотра в рамках записей, относящихся к соответствующему муниципальному образованию
            "куратор омсу"
                or "подписант омсу" => false,

            // Доступ для ведения в части типов:
            // - Исполнительный орган государственной власти;
            // - Орган местного самоуправления;
            // - Ветеринарная клиника: государственная
            "оператор ветслужбы" => organization?.Type.Name.ToLower() is
                null
                or "исполнительный орган государственной власти"
                or "орган местного самоуправления"
                or "ветеринарная клиника: государственная",

            // Доступ для ведения в рамках записей, относящихся к соответствующему муниципальному образованию, в составе следующих типов:
            // - Приют;
            // - Организация по отлову;
            // - Организация по отлову и приют;
            // - Организация по транспортировке;
            // - Ветеринарная клиника: государственная;
            // - Ветеринарная клиника: частная;
            // - Благотворительный фонд;
            // - Организации по продаже товаров и предоставлению услуг для животных
            "оператор омсу" => (organization == null || user.Locality.Id == organization.Locality.Id) && organization?.Type.Name.ToLower() is
                null
                or "приют"
                or "организация по отлову"
                or "организация по отлову и приют"
                or "организация по транспортировке"
                or "ветеринарная клиника: государственная"
                or "ветеринарная клиника: частная"
                or "благотворительный фонд"
                or "организации по продаже товаров и предоставлению услуг для животных",
            
            "гость" => false,
            
            "администратор" => true,
            
            _ => throw new Exception($"Неизвестная роль: {user.Role.Name}")
        };
    }

    public static ISet<string> GetAllowedEntryTypesToAdd(this AppUser user)
    {
        return user.Role.Name.ToLower() switch
        {
            // Доступ для поиска и просмотра в рамках всех записей реестра
            "куратор ветслужбы"
                or "куратор по отлову"
                or "куратор приюта"
                or "подписант ветслужбы"
                or "подписант по отлову"
                or "подписант приюта" => new HashSet<string>(),

            // Доступ для поиска и просмотра в рамках записей, относящихся к соответствующему муниципальному образованию
            "куратор омсу"
                or "подписант омсу" => new HashSet<string>(),

            // Доступ для ведения в части типов:
            // - Исполнительный орган государственной власти;
            // - Орган местного самоуправления;
            // - Ветеринарная клиника: государственная
            "оператор ветслужбы" => new HashSet<string>
            {
                "Исполнительный орган государственной власти",
                "Орган местного самоуправления",
                "Ветеринарная клиника: государственная",
            },

            // Доступ для ведения в рамках записей, относящихся к соответствующему муниципальному образованию, в составе следующих типов:
            // - Приют;
            // - Организация по отлову;
            // - Организация по отлову и приют;
            // - Организация по транспортировке;
            // - Ветеринарная клиника: государственная;
            // - Ветеринарная клиника: частная;
            // - Благотворительный фонд;
            // - Организации по продаже товаров и предоставлению услуг для животных
            "оператор омсу" => new HashSet<string>
            {
                "Приют",
                "Организация по отлову",
                "Организация по отлову и приют",
                "Организация по транспортировке",
                "Ветеринарная клиника: государственная",
                "Ветеринарная клиника: частная",
                "Благотворительный фонд",
                "Организации по продаже товаров и предоставлению услуг для животных",
            },
            
            "гость" => new HashSet<string>(),
            
            "администратор" => new AllContainingHashSet<string>(),
            
            _ => throw new Exception($"Неизвестная роль: {user.Role.Name}")
        };
    }

    public static ISet<string> GetAllowedLocalitiesToAdd(this AppUser user)
    {
        return user.Role.Name.ToLower() switch
        {
            // Доступ для поиска и просмотра в рамках всех записей реестра
            "куратор ветслужбы"
                or "куратор по отлову"
                or "куратор приюта"
                or "подписант ветслужбы"
                or "подписант по отлову"
                or "подписант приюта" => new AllContainingHashSet<string>(),

            // Доступ для поиска и просмотра в рамках записей, относящихся к соответствующему муниципальному образованию
            "куратор омсу"
                or "подписант омсу" => new HashSet<string>
                {
                    user.Locality.Name
                },

            // Доступ для ведения в части типов:
            // - Исполнительный орган государственной власти;
            // - Орган местного самоуправления;
            // - Ветеринарная клиника: государственная
            "оператор ветслужбы" => new AllContainingHashSet<string>(),

            // Доступ для ведения в рамках записей, относящихся к соответствующему муниципальному образованию, в составе следующих типов:
            // - Приют;
            // - Организация по отлову;
            // - Организация по отлову и приют;
            // - Организация по транспортировке;
            // - Ветеринарная клиника: государственная;
            // - Ветеринарная клиника: частная;
            // - Благотворительный фонд;
            // - Организации по продаже товаров и предоставлению услуг для животных
            "оператор омсу" => new HashSet<string>
            {
                user.Locality.Name
            },
            
            "гость" => new HashSet<string>(),
            
            "администратор" => new AllContainingHashSet<string>(),
            
            _ => throw new Exception($"Неизвестная роль: {user.Role.Name}")
        };
    }
}

file class AllContainingHashSet<T> : ISet<T>
{
    public IEnumerator<T> GetEnumerator()
    {
        yield break;
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
    }

    void ICollection<T>.Add(T item)
    {
        throw new Exception("Can't add item to AllContainingHashSet");
    }

    public void ExceptWith(IEnumerable<T> other)
    {
        throw new Exception("Can't call ExceptWith in AllContainingHashSet");
    }

    public void IntersectWith(IEnumerable<T> other)
    {
        throw new Exception("Can't call IntersectWith in AllContainingHashSet");
    }

    public bool IsProperSubsetOf(IEnumerable<T> other)
    {
        throw new Exception("Can't call IsProperSubsetOf in AllContainingHashSet");
    }

    public bool IsProperSupersetOf(IEnumerable<T> other)
    {
        throw new Exception("Can't call IsProperSupersetOf in AllContainingHashSet");
    }

    public bool IsSubsetOf(IEnumerable<T> other)
    {
        throw new Exception("Can't call IsSubsetOf in AllContainingHashSet");
    }

    public bool IsSupersetOf(IEnumerable<T> other)
    {
        throw new Exception("Can't call IsSupersetOf in AllContainingHashSet");
    }

    public bool Overlaps(IEnumerable<T> other)
    {
        throw new Exception("Can't call Overlaps in AllContainingHashSet");
    }

    public bool SetEquals(IEnumerable<T> other)
    {
        return other is AllContainingHashSet<T>;
    }

    public void SymmetricExceptWith(IEnumerable<T> other)
    {
        throw new Exception("Can't call SymmetricExceptWith in AllContainingHashSet");
    }

    public void UnionWith(IEnumerable<T> other)
    {
        throw new Exception("Can't call UnionWith in AllContainingHashSet");
    }

    bool ISet<T>.Add(T item)
    {
        throw new Exception("Can't add item to AllContainingHashSet");
    }

    public void Clear()
    {
        throw new Exception("Can't clear AllContainingHashSet");
    }

    public bool Contains(T item)
    {
        return true;
    }

    public void CopyTo(T[] array, int arrayIndex)
    {
        throw new Exception("Can't call CopyTo in AllContainingHashSet");
    }

    public bool Remove(T item)
    {
        throw new Exception("Can't remove items from AllContainingHashSet");
    }

    public int Count => int.MaxValue;
    public bool IsReadOnly => true;
}