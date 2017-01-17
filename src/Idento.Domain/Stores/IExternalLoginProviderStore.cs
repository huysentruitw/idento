﻿/*
 * Copyright (c) Wouter Huysentruit
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
using System.Threading.Tasks;
using Idento.Domain.Models;

namespace Idento.Domain.Stores
{
    public interface IExternalLoginProviderStore
    {
        Task<ExternalLoginProvider> GetById(Guid id);
        Task<IList<ExternalLoginProvider>> GetAll();
        Task<IList<ExternalLoginProvider>> GetEnabled();
        Task<int> Create(ExternalLoginProvider entity);
        Task<int> Update(ExternalLoginProvider entity);
        Task<ExternalLoginProvider> Delete(Guid id);
    }
}