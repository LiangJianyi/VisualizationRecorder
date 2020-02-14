﻿#pragma once

#include "MainPage.g.h"

namespace winrt::TxtRecordGenerator::implementation
{
    struct MainPage : MainPageT<MainPage>
    {
        MainPage();
    };
}

namespace winrt::TxtRecordGenerator::factory_implementation
{
    struct MainPage : MainPageT<MainPage, implementation::MainPage>
    {
    };
}
