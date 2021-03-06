﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RestfulieClient.resources
{
    public interface IRemoteResourceService
    {
        object Execute(string uri, string transitionName);
        object GetResourceFromWeb(string uri);
        dynamic Get();

        dynamic Create(string content);
    } 
}
