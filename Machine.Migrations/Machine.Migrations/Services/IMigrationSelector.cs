﻿using System;
using System.Collections.Generic;

namespace Machine.Migrations.Services
{
  public interface IMigrationSelector
  {
    ICollection<Migration> SelectMigrations();
  }
}