using AdventOfCode2020.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AdventOfCode2020.Days
{
    [Solution(21)]
    class Day21 : ISolution
    {
        private class Recipe
        {
            public List<string> Ingredients = new List<string>();
            public List<string> Allergens = new List<string>();
        }

        private static List<Recipe> ParseInput(string input)
        {
            return input.Lines().Select(line =>
            {
                Recipe recipe = new Recipe();
                string[] words = line.Split(' ');
                int i = 0;
                for (; i < words.Length; ++i)
                {
                    if (words[i] == "(contains")
                    {
                        ++i;
                        break;
                    }
                    recipe.Ingredients.Add(words[i]);
                }
                for (; i < words.Length; ++i)
                {
                    recipe.Allergens.Add(words[i][0..^1]);
                }
                return recipe;
            }).ToList();
        }

        private Dictionary<string, HashSet<string>> MatchAllergensToIngredients(List<Recipe> recipes)
        {
            Dictionary<string, HashSet<string>> allergens = new Dictionary<string, HashSet<string>>();
            foreach (Recipe recipe in recipes)
            {
                foreach (string allergen in recipe.Allergens)
                {
                    // New allergen to look for
                    if (!allergens.ContainsKey(allergen))
                    {
                        HashSet<string> potential = new HashSet<string>(recipe.Ingredients);
                        foreach (Recipe otherRecipe in recipes)
                        {
                            if (otherRecipe.Allergens.Contains(allergen))
                            {
                                potential.IntersectWith(otherRecipe.Ingredients);
                            }
                        }

                        // Add allergen and ingredients containing the allergen
                        allergens.Add(allergen, potential);
                    }
                }
            }
            return allergens;
        }

        public object PartA(string input)
        {
            List<Recipe> recipes = ParseInput(input);
            Dictionary<string, HashSet<string>> allergens = MatchAllergensToIngredients(recipes);

            HashSet<string> harmfulIngredients = allergens.Values.SelectMany(ingrs => ingrs).ToHashSet();
            return recipes.Aggregate(0, (sum, recipe) => sum + recipe.Ingredients.Count(ingr => !harmfulIngredients.Contains(ingr)));
        }

        public object PartB(string input)
        {
            List<Recipe> recipes = ParseInput(input);
            Dictionary<string, HashSet<string>> allergens = MatchAllergensToIngredients(recipes);
            Dictionary<string, string> isolatedAllergens = new Dictionary<string, string>();

            while (allergens.Count > 0)
            {
                foreach ((string allergen, HashSet<string> ingredients) in allergens)
                {
                    // Remove previously isolated ingredients
                    ingredients.RemoveWhere(ingr => isolatedAllergens.ContainsKey(ingr));

                    if (ingredients.Count == 1)
                    {
                        string ingr = ingredients.First();
                        isolatedAllergens.Add(ingredients.First(), allergen);
                        allergens.Remove(allergen);
                    }
                }

            }

            return string.Join(',', isolatedAllergens.OrderBy(pair => pair.Value).Select(pair => pair.Key));
        }
    }
}
