/*
 * This file is part of the Buildings and Habitats object Model (BHoM)
 * Copyright (c) 2015 - 2024, the respective contributors. All rights reserved.
 *
 * Each contributor holds copyright over their respective contributions.
 * The project versioning (Git) records all such contribution source information.
 *                                           
 *                                                                              
 * The BHoM is free software: you can redistribute it and/or modify         
 * it under the terms of the GNU Lesser General Public License as published by  
 * the Free Software Foundation, either version 3.0 of the License, or          
 * (at your option) any later version.                                          
 *                                                                              
 * The BHoM is distributed in the hope that it will be useful,              
 * but WITHOUT ANY WARRANTY; without even the implied warranty of               
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the                 
 * GNU Lesser General Public License for more details.                          
 *                                                                            
 * You should have received a copy of the GNU Lesser General Public License     
 * along with this code. If not, see <https://www.gnu.org/licenses/lgpl-3.0.html>.      
 */

using BH.Engine.Base;
using BH.Engine.Units;
using BH.oM.Base;
using BH.oM.LifeCycleAssessment;
using BH.oM.LifeCycleAssessment.Fragments;
using BH.oM.LifeCycleAssessment.MaterialFragments;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

using BH.oM.Adapters.Materials2050;
using BH.oM.Materials2050;
using BH.oM.Materials2050.Fragments;

namespace BH.Adapter.Materials2050
{
    public static partial class Convert
    {
        /***************************************************/
        /****           Public Methods                  ****/
        /***************************************************/

        public static EnvironmentalProductDeclaration ToEnvironmentalProductDeclaration(this CustomObject obj, CustomObject resultObj, Materials2050Config config)
        {
            EnvironmentalProductDeclaration epd = new EnvironmentalProductDeclaration();
            List<EnvironmentalProductDeclaration> epdList = new List<EnvironmentalProductDeclaration>();

            // CustomObjects will vary based on which API is used for the query
            // Only OpenAPI is implemented right now
            if (config != null)
            {
                Enum apiT = config.APIName;
                switch (apiT)
                {
                    case APIName.Undefined:
                        {
                            BH.Engine.Base.Compute.RecordError($"No objects were pulled. {apiT} API was used for the query.");
                            break;
                        }
                    case APIName.OpenAPI:
                        {
                            epd = fromOpenAPI(obj, resultObj, config);
                            break;
                        }
                    case APIName.GenericMaterialsAPI:
                        {
                            BH.Engine.Base.Compute.RecordError($"The {apiT} API has not yet been implemented, no objects were created from the API query. Please use the OpenAPI.");
                            break;
                        }
                    case APIName.ProductAPI:
                        {
                            BH.Engine.Base.Compute.RecordError($"The {apiT} API has not yet been implemented, no objects were created from the API query. Please use the OpenAPI.");
                            break;
                        }
                    case APIName.GlobalWarmingAPI:
                        {
                            BH.Engine.Base.Compute.RecordError($"The {apiT} API has not yet been implemented, no objects were created from the API query. Please use the OpenAPI.");
                            break;
                        }
                    default:
                        break;
                }
                return epd;
            }
            else
            {
                BH.Engine.Base.Compute.RecordError("Error.");
                return null;
            }
        }

        /***************************************************/

        public static EnvironmentalProductDeclaration fromOpenAPI(CustomObject obj, CustomObject resultObj, Materials2050Config config)
        {
            EnvironmentalProductDeclaration epd = new EnvironmentalProductDeclaration();
            // OpenAPI call object schema

            // basic results parsing data
            ResultsPagination resultsPagination = new ResultsPagination();
            resultsPagination.APIName = config.APIName;
            resultsPagination.TotalProducts = (int)(obj.PropertyValue("TotalProducts") ?? 0);
            resultsPagination.ProductsOnPage = (int)(obj.PropertyValue("countProductsOnPage") ?? 0);
            resultsPagination.CurrentPage = (int)(obj.PropertyValue("current_page") ?? 0);
            resultsPagination.ProductRange = obj.PropertyValue("product_range")?.ToString() ?? string.Empty;

            // Not implemented - no use case provided - inaccessible URLs
            //string next = co.PropertyValue("next")?.ToString();
            //string previous = co.PropertyValue("previous")?.ToString();

            // Add additional data where possible 
            // TODO - The LCA oM fragment is not very well structured and would require significant refactor to be helpful here
            AdditionalEPDData addData = new AdditionalEPDData();
            addData.Manufacturer = resultObj.CustomData["company"]?.ToString() ?? "No Manufacturer recorded";
            addData.Description = resultObj.CustomData["product_type"]?.ToString() ?? "No product type recorded.";
            addData.Description += $" - {resultObj.CustomData["material_type"]?.ToString() ?? "No material type recorded."}";
            addData.Manufacturer += $" - {resultObj.CustomData["manufacturing_location"]?.ToString() ?? "No Manufacturer location recorded"}";

            epd.Name = resultObj.CustomData["name"]?.ToString() ?? null;

            // Material Facts object within the Results Object
            CustomObject materialObj = resultObj.CustomData["material_facts"] as CustomObject;

            double unused = double.NaN;
            double a1toa3 = double.NaN;

            var value = materialObj.CustomData["manufacturing"];
            if(value != null)
            {
                try
                {
                    a1toa3 = (double)materialObj.CustomData["manufacturing"];
                } 
                catch //(Exception ex)
                {
                    // This warning is important to an extent, but the UX is very unhelpful. Would be better to be returned as a log in the future. For now all missing values are returned as double.NaN
                    // BH.Engine.Base.Compute.RecordWarning(ex, $"A3 Values were not found for {epd.Name}. Please verify your results."); 
                }
            } else
            {
                a3 = double.NaN;
            }

            ClimateChangeTotalMetric metric = new ClimateChangeTotalMetric(unused, unused, unused, a1toa3, unused, unused, unused, unused, unused, unused, unused, unused, unused, unused, unused, unused, unused, unused, unused, unused);

            epd.QuantityType = GetQuantityTypeFromString(materialObj.CustomData["declared_unit"] as string);
            epd.EnvironmentalMetrics.Add(metric);

            // Add data fragments
            EnvironmentalProductDeclaration epdData = (EnvironmentalProductDeclaration)Modify.AddFragment(epd, addData);
            EnvironmentalProductDeclaration epdDataNav = (EnvironmentalProductDeclaration)Modify.AddFragment(epdData, resultsPagination);

            return epdDataNav;
        }

        /***************************************************/

        public static QuantityType GetQuantityTypeFromString(string unitFrom)
        {
            switch (unitFrom)
            {
                case "":
                case null:
                    return QuantityType.Undefined;
                case "piece":
                    return QuantityType.Item;
                case "yd3":
                case "y3":
                case "yd³":
                case "y³":
                case "m3":
                case "m³":
                    return QuantityType.Volume;
                case "t":
                case "short ton":
                case "tonne":
                case "metric ton":
                case "lb":
                case "kg":
                    return QuantityType.Mass;
                case "sq ft":
                case "sqft":
                case "ft2":
                case "square ft":
                case "SF":
                case "m2":
                case "M2":
                case "sq m":
                case "m²":
                    return QuantityType.Area;
                case "in":
                case "inches":
                case "ft":
                case "feet":
                case "m":
                case "meters":
                case "metres":
                    return QuantityType.Length;
            }
            return QuantityType.Undefined;
        }

        /***************************************************/

        public static double ConvertToSI(double val, string unitTo)
        {
            double unitMult = 1;
            switch (unitTo)
            {
                case "kg/m3":
                case "kg/m³":
                    unitMult = 1;
                    break;
                case "lb/y3":
                    unitMult = 1.68555;
                    break;
                case "yd3":
                case "y3":
                case "yd³":
                case "y³":
                    return val.ToCubicYard();
                case "short ton":
                    unitMult = 0.00110231;
                    break;
                case "tonne":
                case "t":
                case "metric ton":
                    unitMult = 0.001;
                    break;
                case "lb":
                case "pound":
                    return val.ToPoundForce();
                case "sq ft":
                case "sqft":
                case "ft2":
                case "square ft":
                case "SF":
                    unitMult = 10.7639;
                    break;
                case "in":
                case "inches":
                    return val.ToInch();
                case "ft":
                case "feet":
                    return val.ToFoot();
            }
            double valueSI = unitMult * val;
            if (valueSI == 0)
            { valueSI = double.NaN; }
            return valueSI;
        }

        /***************************************************/

        public static double GetValFromString(this string str)
        {
            Match match = Regex.Match(str, "[A-Za-z\\s]");
            string numString = str.Substring(0, match.Index);
            double val = double.NaN;
            Double.TryParse(numString, out val);
            if (numString == "")
            { val = 1; }
            return val;
        }

        /***************************************************/

        public static string GetUnitsFromString(this string str)
        {
            Match match = Regex.Match(str, "[A-Za-z]");
            string units = str.Substring(match.Index);
            return units;
        }

    }
}