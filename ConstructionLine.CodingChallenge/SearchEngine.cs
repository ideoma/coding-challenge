using System;
using System.Collections.Generic;
using System.Linq;

namespace ConstructionLine.CodingChallenge
{
    public class SearchEngine
    {
        private readonly Dictionary<(Guid ColorId, Guid SizeId), List<Shirt>> _store;

        public SearchEngine(List<Shirt> shirts)
        {
            if (shirts == null) throw new ArgumentNullException(nameof(shirts));
            
            _store = shirts.ToLookup(shirt => (shirt.Color.Id, shirt.Size.Id))
                .ToDictionary(group => group.Key, group => group.ToList());
        }

        public SearchResults Search(SearchOptions options)
        {
            if (options?.Colors == null || options.Sizes == null) throw new ArgumentNullException(nameof(options));
            
            var shirtsFound = new List<Shirt>();
            // Count of found items
            Dictionary<Guid, (Size Size, int Count)> sizeCounts = Size.All.ToDictionary(s => s.Id, s => (s, 0));
            Dictionary<Guid, (Color Color, int Count)> colorCounts = Color.All.ToDictionary(c => c.Id, c => (c, 0));

            // No search colors === all colors
            var optionsColors = options.Colors.Any() ? options.Colors : Color.All;
            // No search sizes === all sizes
            var optionsSizes = options.Sizes.Any() ? options.Sizes : Size.All;
            
            foreach (var color in optionsColors)
            {
                foreach (var size in optionsSizes)
                {
                    if (_store.TryGetValue((color.Id, size.Id), out var match))
                    {
                        shirtsFound.AddRange(match);
                        Increment(sizeCounts, size.Id, match.Count);
                        Increment(colorCounts, color.Id, match.Count);
                    }
                }
            }

            return new SearchResults
            {
                Shirts = shirtsFound,
                ColorCounts = colorCounts.Values.Select(value => new ColorCount
                {
                    Color = value.Color, Count = value.Count
                }).ToList(),
                SizeCounts = sizeCounts.Values.Select(value => new SizeCount
                {
                    Size = value.Size, Count = value.Count
                }).ToList()
            };
        }

        private void Increment<T>(Dictionary<Guid, (T Item, int Count)> dict, Guid key, int count)
        {
            if (dict.TryGetValue(key, out var existingCount))
            {
                dict[key] = (existingCount.Item, existingCount.Count + count);
            }
        }
    }
}