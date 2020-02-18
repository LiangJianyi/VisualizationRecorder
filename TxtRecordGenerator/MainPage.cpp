﻿#include "pch.h"
#include "MainPage.h"
#include "MainPage.g.cpp"
#include "../../SundryUtilty/Cpp code files/JanyeeDateTime/DateTime.h"
#include "MainPageHelper.h"

using namespace winrt;
using namespace Windows::UI::Xaml;
using namespace Windows::Foundation;

namespace winrt::TxtRecordGenerator::implementation
{
	MainPage::MainPage() {
		InitializeComponent();
		UpdateLayout();
	}

	void MainPage::UpdateLayout() {
		OutputDebugStringA((std::string("GeneratorButton().ActualWidth(): ") + std::to_string(GeneratorButton().ActualWidth()) + "\n").c_str());
		OutputDebugStringA((std::string("EntriesTotal().ActualWidth(): ") + std::to_string(EntriesTotal().ActualWidth()) + "\n").c_str());
		//EntriesTotal().Width(GeneratorButton().ActualWidth());
	}
}


void winrt::TxtRecordGenerator::implementation::MainPage::GeneratorButton_Click(winrt::Windows::Foundation::IInspectable const&, winrt::Windows::UI::Xaml::RoutedEventArgs const&) {
	IReference<DateTime> beginDateRef = BeginDatePicker().Date();
	IReference<DateTime> endDateRef = EndDatePicker().Date();
	if (beginDateRef != nullptr && endDateRef != nullptr) {
		// DateTimePick 返回的时钟震荡周期很奇怪，与std::chrono::system_clock::now()的周期大约相差 11644473596UL，
		// 目前原因尚不清楚。
		// 通过乘以 std::chrono::system_clock::period::num / std::chrono::system_clock::period::den  - 11644473596UL
		// 转化为实际的秒数
		time_t bt = beginDateRef.Value().time_since_epoch().count() * std::chrono::system_clock::period::num / std::chrono::system_clock::period::den;
		bt = static_cast<time_t>(static_cast<uint64_t>(bt) - 11644473596UL);
		time_t et = endDateRef.Value().time_since_epoch().count() * std::chrono::system_clock::period::num / std::chrono::system_clock::period::den;
		et = static_cast<time_t>(static_cast<uint64_t>(et) - 11644473596UL);

		const Janyee::DateTime& beginDateTime = Janyee::DateTime(bt);
		const Janyee::DateTime& endDateTime = Janyee::DateTime(et);
		OutputDebugStringA(("beginDateTime: " + beginDateTime.ToString() + "\n").c_str());
		OutputDebugStringA(("endDateTime: " + endDateTime.ToString() + "\n").c_str());
		for (uint32_t i = 0; i < EntriesTotal().Value(); i++) {
			try {
				const auto& entry = MainPageHelper::RandomDateTime(beginDateTime, endDateTime);
				std::stringstream s;
				s << entry.ToShortDate()
					<< " x"
					<< std::to_string(
						static_cast<int>(MainPageHelper::RandomEventFrequency(EventFrequencyDownLimit().Value(), EventFrequencyUpperLimit().Value()))
					)
					<< "\n";
				OutputDebugStringA(s.str().c_str());
			}
			catch (const Janyee::DateTimeException & e) {
				OutputDebugStringA(e.what());
			}
		}
	}
	else {
		OutputDebugStringA("You not pick a date.\n");
	}
}
