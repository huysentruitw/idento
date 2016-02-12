/*
 * Copyright 2016 Wouter Huysentruit
 *
 * Licensed under the Apache License, Version 2.0 (the "License");
 * you may not use this file except in compliance with the License.
 * You may obtain a copy of the License at
 *
 *   http://www.apache.org/licenses/LICENSE-2.0
 *
 * Unless required by applicable law or agreed to in writing, software
 * distributed under the License is distributed on an "AS IS" BASIS,
 * WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
 * See the License for the specific language governing permissions and
 * limitations under the License.
 */

using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Reflection;
using Microsoft.AspNet.Mvc.Rendering;

namespace Idento.Helpers
{
    public static class EnumExtensions
    {
        /// <summary>
        /// Gets the first available display attribute of the enumeration.
        /// </summary>
        /// <param name="enumeration">The enumeration.</param>
        /// <returns>Returns the first display attribute of the enumeration when available, otherwise null.</returns>
        private static DisplayAttribute GetFirstDisplayAttribute(this Enum enumeration)
        {
            var memberInfos = enumeration.GetType().GetMember(enumeration.ToString());
            return memberInfos?.FirstOrDefault()?.GetCustomAttributes<DisplayAttribute>(false).FirstOrDefault();
        }

        /// <summary>
        /// Gets the display name of the enumeration. This is the enumeration name but can be overridden by using [Display(Name="Bogus")].
        /// </summary>
        /// <param name="enumeration">The enumeration.</param>
        /// <returns>The display name of the enumeration.</returns>
        public static string GetDisplayName(this Enum enumeration)
        {
            var displayAttribute = GetFirstDisplayAttribute(enumeration);
            return !string.IsNullOrWhiteSpace(displayAttribute?.Name) ? displayAttribute.Name : enumeration.ToString();
        }

        /// <summary>
        /// Gets the display group name of the enumeration. This defaults to an empty string but can be overriden by using [Display(GroupName="MyGroup")].
        /// </summary>
        /// <param name="enumeration">The enumeration.</param>
        /// <returns>The display group name of the enumeration.</returns>
        public static string GetDisplayGroupName(this Enum enumeration)
        {
            var displayAttribute = GetFirstDisplayAttribute(enumeration);
            return !string.IsNullOrWhiteSpace(displayAttribute?.GroupName) ? displayAttribute.GroupName : string.Empty;
        }

        /// <summary>
        /// Cache that keeps SelectListGroup instances, so enumerations from same type and groupname are actually grouped together.
        /// </summary>
        private static Dictionary<Guid, Dictionary<string, SelectListGroup>> listGroupCache = new Dictionary<Guid, Dictionary<string, SelectListGroup>>();
        /// <summary>
        /// Converts an enumeration to a SelectListGroup object. The groupname is retreived from the DisplayAttribute on the enumeration.
        /// </summary>
        /// <param name="enumeration">The enumeration.</param>
        /// <returns>The (cached) SelectListGroup for this type and groupname combination.</returns>
        private static SelectListGroup ToSelectListGroup(this Enum enumeration)
        {
            var groupName = GetDisplayGroupName(enumeration);
            var typeInfo = enumeration.GetType().GetTypeInfo();
            if (!listGroupCache.ContainsKey(typeInfo.GUID))
                listGroupCache.Add(typeInfo.GUID, new Dictionary<string, SelectListGroup>());
            if (!listGroupCache[typeInfo.GUID].ContainsKey(groupName))
                listGroupCache[typeInfo.GUID][groupName] = new SelectListGroup { Name = groupName };
            return listGroupCache[typeInfo.GUID][groupName];
        }

        /// <summary>
        /// Create a SelectListItem from an enumeration.
        /// </summary>
        /// <param name="enumeration">The enum type.</param>
        /// <param name="useNameAsItemValue">Determines what to use as SelectListItem.Value. True will use the enumeration name, false will use the enumeration value.</param>
        /// <returns>The SelectListItem.</returns>
        public static SelectListItem ToSelectListItem(this Enum enumeration, bool useNameAsItemValue = false)
        {
            string text = GetDisplayName(enumeration);
            string value = useNameAsItemValue ? enumeration.ToString() : Convert.ToInt32(enumeration).ToString();
            return new SelectListItem
            {
                Text = text,
                Value = value,
                Group = enumeration.ToSelectListGroup()
            };
        }

        /// <summary>
        /// Create a list of SelectListItem's from an enum type.
        /// </summary>
        /// <param name="enumType">The enum type.</param>
        /// <param name="useNameAsItemValue">Determines what to use as SelectListItem.Value. True will use the enumeration name, false will use the enumeration value.</param>
        /// <returns>IEnumerable of SelectListItem's.</returns>
        public static IEnumerable<SelectListItem> ToSelectList<T>(bool useNameAsItemValue = false) where T : struct
        {
            var enumType = typeof(T);
            if (!enumType.GetTypeInfo().IsEnum)
                throw new ArgumentException("Not an enum type");
            return Enum.GetValues(enumType).Cast<Enum>().Select(value => value.ToSelectListItem(useNameAsItemValue));
        }
    }
}
