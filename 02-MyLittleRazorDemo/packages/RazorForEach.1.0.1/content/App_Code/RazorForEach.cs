using System;
using System.Collections.Generic;
using System.Web.WebPages;

public class IndexedItem<TModel> {
    public IndexedItem(int index, TModel item) {
        Index = index;
        Item = item;
    }

    public int Index { get; private set; }
    public TModel Item { get; private set; }
}

public static class HaackHelpers {
    public static HelperResult Each<TItem>(this IEnumerable<TItem> items, Func<IndexedItem<TItem>, HelperResult> template) {
        return new HelperResult(writer => {
            int index = 0;

            foreach (var item in items) {
                var result = template(new IndexedItem<TItem>(index++, item));
                result.WriteTo(writer);
            }
        });
    }
}
