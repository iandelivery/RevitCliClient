// SPDX-License-Identifier: MIT
using System.Threading.Tasks;

namespace RevitCliClient.Abstractions
{
    public delegate Task<int> SendCommandFunc(string command, object? parameters);
}
