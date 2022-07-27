using customerportalapi.Entities;
using customerportalapi.Entities.Enums;
using System;
using System.Reflection;
using System.Linq;
using System.Collections.Generic;

namespace customerportalapi.Services
{
    public static class UserInvitationUtils
    {
        public static string GetLanguage(string invitationLanguage)
        {
            if (invitationLanguage == null)
                return LanguageTypes.en.ToString();
            switch (invitationLanguage.ToLower().Trim())
            {
                case "spanish":
                case "es":
                    return LanguageTypes.es.ToString();
                case "english":
                case "en":
                    return LanguageTypes.en.ToString();
                case "portuguese":
                case "pt":
                    return LanguageTypes.pt.ToString();
                case "french":
                case "fr":
                    return LanguageTypes.fr.ToString();
                default:
                    return LanguageTypes.en.ToString();
            }
        }

        public static int GetUserType(string invitationCustomerType)
        {
            if (invitationCustomerType == null)
                return (int)UserTypes.Residential;
            switch (invitationCustomerType.ToLower().Trim())
            {
                case "residential":
                case "0":
                    return (int)UserTypes.Residential;
                case "business":
                case "1":
                    return (int)UserTypes.Business;
                default:
                    return (int)UserTypes.Residential;
            }
        }

        public static string GetAccountType(int userType)
        {
            switch (userType)
            {
                case (int)UserTypes.Residential:
                    return AccountType.Residential;
                case (int)UserTypes.Business:
                    return AccountType.Business;
                default:
                    return AccountType.Residential;
            }
        }

        public static InvitationMandatoryData InitInvitationData()
        {
            var data = new InvitationMandatoryData
            {
                ContactUsername = GetMandatoryData(SystemTypes.CRM, EntityNames.contacts, null, StateEnum.Unchecked),
                Contract = GetMandatoryData(SystemTypes.CRM, EntityNames.iav_contracts, null, StateEnum.Unchecked),
                SmContractCode = GetMandatoryData(SystemTypes.CRM, EntityNames.iav_contracts, null, StateEnum.Unchecked),
                SMContract = GetMandatoryData(SystemTypes.SM, EntityNames.WBSGetContract, null, StateEnum.Unchecked),
                Leaving = GetMandatoryData(SystemTypes.SM, EntityNames.WBSGetContract, null, StateEnum.Checked),
                ActiveContract = GetMandatoryData(SystemTypes.SM, EntityNames.WBSGetContract, null, StateEnum.Unchecked),

                UnitPassword = GetMandatoryData(SystemTypes.CRM, EntityNames.WBSGetContract, null, StateEnum.Unchecked),
                UnitName = GetMandatoryData(SystemTypes.CRM, EntityNames.iav_contracts, null, StateEnum.Unchecked),
                UnitSizeCode = GetMandatoryData(SystemTypes.CRM, EntityNames.iav_contracts, null, StateEnum.Unchecked), // TODO: check EntityNames
                ContractStoreCode = GetMandatoryData(SystemTypes.CRM, EntityNames.iav_contracts, null, StateEnum.Unchecked),
                StoreCode = GetMandatoryData(SystemTypes.CRM, EntityNames.iav_stores, null, StateEnum.Unchecked),
                OpeningDaysFirst = GetMandatoryData(SystemTypes.CRM, EntityNames.iav_stores, null, StateEnum.Unchecked),
                OpeningDaysLast = GetMandatoryData(SystemTypes.CRM, EntityNames.iav_stores, null, StateEnum.Unchecked),
                OpeningHoursFrom = GetMandatoryData(SystemTypes.CRM, EntityNames.iav_stores, null, StateEnum.Unchecked),
                OpeningHoursTo = GetMandatoryData(SystemTypes.CRM, EntityNames.iav_stores, null, StateEnum.Unchecked),
                StoreName = GetMandatoryData(SystemTypes.CRM, EntityNames.iav_stores, null, StateEnum.Unchecked),
                StoreEmail = GetMandatoryData(SystemTypes.CRM, EntityNames.iav_stores, null, StateEnum.Unchecked),
                StoreCity = GetMandatoryData(SystemTypes.CRM, EntityNames.iav_stores, null, StateEnum.Unchecked),
                ContractOpportunity = GetMandatoryData(SystemTypes.CRM, EntityNames.iav_contracts, null, StateEnum.Unchecked),
                OpportunityId = GetMandatoryData(SystemTypes.CRM, EntityNames.opportunities, null, StateEnum.Unchecked),
                ExpectedMoveIn = GetMandatoryData(SystemTypes.CRM, EntityNames.opportunities, null, StateEnum.Unchecked),


                SiteMailType = GetMandatoryData(SystemTypes.CRM, EntityNames.iav_stores, null, StateEnum.Unchecked),
                UnitColour = GetMandatoryData(SystemTypes.CRM, EntityNames.products, null, StateEnum.Unchecked),
                UnitCorridor = GetMandatoryData(SystemTypes.CRM, EntityNames.products, null, StateEnum.Unchecked),
                UnitExceptions = GetMandatoryData(SystemTypes.CRM, EntityNames.products, null, StateEnum.Unchecked),
                UnitFloor = GetMandatoryData(SystemTypes.CRM, EntityNames.products, null, StateEnum.Unchecked),
                UnitZone = GetMandatoryData(SystemTypes.CRM, EntityNames.products, null, StateEnum.Unchecked),



                /*SiteMailType = GetMandatoryData(SystemTypes.CRM, EntityNames.iav_stores, ((int)StoreMailTypes.WithoutSignageOrNull).ToString(), StateEnum.Checked),
                UnitColour = GetMandatoryData(SystemTypes.CRM, EntityNames.products, null, StateEnum.Checked),
                UnitCorridor = GetMandatoryData(SystemTypes.CRM, EntityNames.products, null, StateEnum.Checked),
                UnitExceptions = GetMandatoryData(SystemTypes.CRM, EntityNames.products, null, StateEnum.Checked),
                UnitFloor = GetMandatoryData(SystemTypes.CRM, EntityNames.products, null, StateEnum.Checked),
                UnitZone = GetMandatoryData(SystemTypes.CRM, EntityNames.products, null, StateEnum.Checked),*/

                InvokedBy = GetMandatoryData(SystemTypes.empty, EntityNames.empty, null, StateEnum.Unchecked),
            };

            return data;
        }

        public static MandatoryData GetMandatoryData(SystemTypes system, EntityNames entity, string value, StateEnum state)
        {
            var data = new MandatoryData()
            {
                Value = value,
                State = state,
                System = system,
                Entity = entity
            };

            return data;
        }

        public static string GetBodyFormattedHideButtonAccessPortal(string body, List<Contract> contracts, User user)
        {
            if (contracts != null && contracts.Count > 1 && user != null && user.Emailverified)
            {
                body = RemoveBlockBetweenStrings(body, "{{ButtonAccessPortalSTART}}", "{{ButtonAccessPortalEND}}");
                body = RemoveBlockBetweenStrings(body, "{{ButtonAccessPortalSTART}}", "{{ButtonAccessPortalEND}}");
                return body;
            }
            else
            {
                return body.Replace("{{ButtonAccessPortalSTART}}", string.Empty).Replace("{{ButtonAccessPortalEND}}", string.Empty); ;
            }
        }

        public static string GetBodyFormatted(EmailTemplate invitationTemplate, User user,
        InvitationMandatoryData fields, string baseURL, string inviteConfirmation)
        {
            string body = invitationTemplate.body;

            PropertyInfo[] properties = typeof(InvitationMandatoryData).GetProperties();
            foreach (var property in properties)
            {
                MandatoryData data = (MandatoryData)property.GetValue(fields);
                if (data != null && !string.IsNullOrEmpty(data.Value))
                {
                    string field = "{{" + property.Name + "}}";

                    switch (property.Name)
                    {
                        case "ExpectedMoveIn":
                            DateTime expectedMoveIn = DateTime.Parse(data.Value);
                            body = body.Replace(field, expectedMoveIn.ToString("dd/MM/yyyy"));
                            expectedMoveIn = expectedMoveIn.Subtract(TimeSpan.FromDays(1));
                            body = body.Replace("{{PreviousExpectedMoveIn}}", expectedMoveIn.ToString("dd/MM/yyyy"));
                            break;

                        case "ContactUsername":
                            body = body.Replace(field, data.Value);
                            body = body.Replace("{{UserPassword}}", user.Password);


                            body = body.Replace("{{UserName}}",
                                string.IsNullOrEmpty(user.Username) ? string.Empty : user.Username);
                            break;

                        case "UnitName":
                            body = body.Replace(field, data.Value);

                            char[] unitName = GetFormatedUnitName(data.Value);

                            body = body.Replace("{{LockCode}}", new string(unitName));
                            break;

                        case "UnitPassword":
                            // Access code password to the unit
                            body = body.Replace("{{Paragraph" + property.Name + "}}", data.Value);
                            break;

                        case "UnitSizeCode":
                            var content = string.Empty;
                            if (invitationTemplate != null && invitationTemplate.Paragraphs != null)
                            {
                                EmailParagraph paragraph = GetParagraphByName(invitationTemplate, property.Name);
                                content = paragraph.CustomContent;
                                if (data.State == StateEnum.Checked)
                                {
                                    content = paragraph.DefaultContent;
                                    content = content.Replace(field, data.Value);
                                }
                            }

                            body = body.Replace("{{Paragraph" + property.Name + "}}", content);
                            break;

                        case "InvokedBy":
                            if (data.Value == ((int)InviteInvocationType.CronJob).ToString())
                            {
                                // only remove magic words
                                body = body.Replace("{{RememberSTART}}", string.Empty)
                                    .Replace("{{RememberEND}}", string.Empty);
                            }
                            else
                            {
                                // Remove all characters between the starting string and the ending string
                                body = RemoveBlockBetweenStrings(body, "{{RememberSTART}}", "{{RememberEND}}");
                            }

                            break;

                        case "SiteMailType":
                            var intSiteMailType = (int)StoreMailTypes.WithoutSignageOrNull;
                            if (!string.IsNullOrEmpty(data.Value))
                                intSiteMailType = Convert.ToInt32(data.Value);
                            switch (intSiteMailType)
                            {
                                case (int)StoreMailTypes.NewSignage:
                                    NewSignageFormat(ref body, fields.UnitFloor.Value, fields.UnitZone.Value, fields.UnitColour.Value, fields.UnitCorridor.Value, fields.UnitExceptions.Value);
                                    break;

                                case (int)StoreMailTypes.OldSignage:
                                    OldSignageFormat(ref body, fields.UnitExceptions.Value, fields.UnitFloor.Value);
                                    break;

                                default:
                                    WithoutSignageOrNullFormat(ref body, fields.UnitExceptions.Value);
                                    break;
                            }

                            break;

                        default:
                            body = body.Replace(field, data.Value);
                            break;
                    }
                }
            }

            body = body.Replace("{{BaseUrl}}", baseURL);
            body = body.Replace("{{InviteConfirmationUrl}}", inviteConfirmation + user.Invitationtoken);

            return body;
        }

        public static char[] GetFormatedUnitName(string data)
        {
            //Si hay una letra al final se cogen los primeros dígitos
            if (IsLastLetter(data))
            {
                data = GetFourLeftLengthString(data);
            }
            //Si hay una letra al inicio se cogen los últimos dígitos
            else
            {
                data = GetFourRightLengthString(data);
            }

            bool firstLetter = IsFirstLetter(data);
            bool lastLetter = IsLastLetter(data);

            //Se cambia la letra por un 0
            if (firstLetter) data = ChangeFirstLetter(data, '0');
            if (lastLetter) data = ChangeLastLetter(data, '0');

            char[] unitName = data.Trim().PadLeft(4, '0').ToCharArray();

            int num;
            if (unitName[0].ToString() != null && (!firstLetter || !lastLetter))
            {
                if (int.TryParse(unitName[0].ToString(), out num))
                {
                    num++;
                    if (num > 9) num = 0;
                    unitName[0] = char.Parse(num.ToString());
                }
            }

            if (unitName[3].ToString() != null && int.TryParse(unitName[3].ToString(), out num))
            {
                num++;
                if (num > 9) num = 0;
                unitName[3] = char.Parse(num.ToString());
            }
            return unitName;
        }

        // Solo muestra Excepciones
        // Oculta: Planta, Zona, ColorZona y Pasillo
        private static void WithoutSignageOrNullFormat(ref string body, string unitExceptions)
        {
            body = RemoveBlockBetweenStrings(body, "{{LocationSTART}}", "{{LocationEND}}");
            ReplaceKeyValueStringEmpty(ref body, "Excepciones", unitExceptions);
        }

        // Muestra: Planta y Excepciones
        // Oculta:  Zona, ColorZona y Pasillo
        private static void OldSignageFormat(ref string body, string unitExceptions, string unitFloor)
        {
            ReplaceKeyValueStringEmpty(ref body, "Planta", unitFloor);
            ReplaceKeyValueStringEmpty(ref body, "Zona", string.Empty);
            ReplaceKeyValueStringEmpty(ref body, "ColorZona", string.Empty);
            ReplaceKeyValueStringEmpty(ref body, "Pasillo", string.Empty);
            ReplaceKeyValueStringEmpty(ref body, "Excepciones", unitExceptions);

            body = body.Replace("{{LocationSTART}}", string.Empty)
                .Replace("{{LocationEND}}", string.Empty);
        }

        // Muestra: Planta, Zona, ColorZona y Pasillo
        // Oculta: Excepciones
        private static void NewSignageFormat(ref string body, string unitFloor, string unitZone, string unitColour, string unitCorridor, string unitExceptions)
        {
            ReplaceKeyValueStringEmpty(ref body, "Planta", unitFloor);
            ReplaceKeyValueStringEmpty(ref body, "Zona", unitZone);
            ReplaceKeyValueStringEmpty(ref body, "ColorZona", unitColour);
            ReplaceKeyValueStringEmpty(ref body, "Pasillo", unitCorridor);
            ReplaceKeyValueStringEmpty(ref body, "Excepciones", unitExceptions);

            body = body.Replace("{{LocationSTART}}", string.Empty)
                .Replace("{{LocationEND}}", string.Empty);
        }

        private static EmailParagraph GetParagraphByName(EmailTemplate template, string name)
        {
            name = name.ToLower();
            if (template != null && template.Paragraphs.Count > 0)
            {
                return template.Paragraphs.Find((t) => t.Name == name);
            }

            return null;
        }

        private static string ChangeFirstNoNumericIfExist(string data, char change)
        {
            char[] dataChar = data.ToCharArray();
            int first;
            bool isNumeric = int.TryParse(dataChar[0].ToString(), out first);
            if (!isNumeric)
            {
                dataChar[0] = change;
                data = new string(dataChar);
            }
            return data;
        }

        private static bool IsFirstLetter(string data)
        {
            char[] dataChar = data.ToCharArray();
            int first;
            bool isNumeric = int.TryParse(dataChar[0].ToString(), out first);
            return !isNumeric;
        }
        private static bool IsLastLetter(string data)
        {
            char[] dataChar = data.ToCharArray();
            int first;
            bool isNumeric = int.TryParse(dataChar[dataChar.Length - 1].ToString(), out first);
            return !isNumeric;
        }

        private static string ChangeFirstLetter(string data, char change)
        {
            char[] dataChar = data.ToCharArray();
            dataChar[0] = change;
            data = new string(dataChar);
            return data;
        }

        private static string ChangeLastLetter(string data, char change)
        {
            char[] dataChar = data.ToCharArray();
            dataChar[dataChar.Length - 1] = change;
            data = new string(dataChar);
            return data;
        }

        private static string GetFourRightLengthString(string st)
        {
            while (st.Length > 4)
            {
                st = st.Remove(0, 1);
            }
            return st;
        }

        private static string GetFourLeftLengthString(string st)
        {
            while (st.Length > 4)
            {
                st = st.Remove(st.Length - 1, 1);
            }
            return st;
        }

        /// <summary>
        /// Replace in Body the "value" string with the "key", and remove entry if the value is empty
        /// </summary>
        /// <param name="body">Body HTML</param>
        /// <param name="key">Key</param>
        /// <param name="value">Value of string</param>
        private static void ReplaceKeyValueStringEmpty(ref string body, string key, string value)
        {
            if (value == null || string.IsNullOrEmpty(value.Trim()))
            {
                body = RemoveBlockBetweenStrings(body, string.Concat("{{Location", key, "START}}"),
                    string.Concat("{{Location", key, "END}}"));
            }
            else
            {
                body = body.Replace(string.Concat("{{", key, "}}"), value)
                    .Replace(string.Concat("{{Location", key, "START}}"), string.Empty)
                    .Replace(string.Concat("{{Location", key, "END}}"), string.Empty);
            }
        }

        /// <summary>
        /// Remove all characters between the starting string and the ending string (both included)
        /// </summary>
        /// <param name="source">String</param>
        /// <param name="start">Starting string</param>
        /// <param name="end">Ending string</param>
        /// <returns>Returns the string without the characters between "start" and "end"</returns>
        public static string RemoveBlockBetweenStrings(string source, string start, string end)
        {
            var result = source;
            if (source.Contains(start) && source.Contains(end))
            {
                var startIndex = source.IndexOf(start, StringComparison.Ordinal);
                var endIndex = source.IndexOf(end, startIndex, StringComparison.Ordinal) + end.Length;
                //result = source.Substring(startIndex, endIndex - startIndex);
                result = source.Remove(startIndex, endIndex - startIndex);
                return result;
            }

            return result;
        }
    }
}
