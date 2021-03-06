﻿using KeyPay.Common.ErrorMessages;
using KeyPay.Data.DatabaseContext;
using KeyPay.Data.Dto.Site.Admin.Users;
using KeyPay.Repo.Infrastructure;
using KeyPay.Services.Site.Admin.Auth.Interface;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace KeyPay.Presentation.Controllers.Site.Admin
{
    [Authorize]
    [ApiExplorerSettings(GroupName = "Site")]
    [Route("site/admin/[controller]")]
    [ApiController]
    public class UsersController : ControllerBase
    {
        public UsersController(IUnitOfWork<KeyPayDbContext> dbContext, AutoMapper.IMapper mapper, IUserService userService)
        {
            _db = dbContext;
            _mapper = mapper;
            _userService = userService;
        }

        private readonly IUnitOfWork<KeyPayDbContext> _db;

        private readonly AutoMapper.IMapper _mapper;

        private readonly IUserService _userService;

        [HttpGet]
        public async Task<IActionResult> GetUsers()
        {
            //var users = await _db.UserRepository.GetAllAsync();

            var users = await _db.UserRepository.GetManyAsync(null, null, "Photoes,BankCards");

            var userToReturn = _mapper.Map<IEnumerable<UserForListDto>>(users);

            return Ok(userToReturn);

        }


        [HttpGet("{id}")]

        public async Task<IActionResult> GetUser(Guid id)
        {
            var user = await _db.UserRepository.GetByIdAsync(id);

            //var user = await _db.UserRepository.GetManyAsync(current => current.Id == id, null, "Photoes,BankCards");

            var userToReturn = _mapper.Map<UserForDetailDto>(user);

            return Ok(userToReturn);
        }



        //  برای زمانی که بخواهیم به یوزر اطلاعات خودش رو بدیم البته باید این اکشن رو در جای دیگری بنویسییم  ولی در کل برای اینکه چطور از 
        //  Claim  
        // میشه استفاده کرد برای این که یک یوزر همان یوزری هست که اطلاعات خودش رو میخواد بگیره
        [Route("getProfileUser/{id}")]
        [HttpGet]

        public async Task<IActionResult> GetProfileUser(Guid id)
        {

            if (User.FindFirst(ClaimTypes.NameIdentifier).Value == id.ToString())
            {
                var user = await _db.UserRepository.GetByIdAsync(id);

                //var user = await _db.UserRepository.GetManyAsync(current => current.Id == id, null, "Photoes,BankCards");

                var userToReturn = _mapper.Map<UserForDetailDto>(user);

                return Ok(userToReturn);
            }

            else
            {
                return Unauthorized("شما دسترسی به این اطلاعات ندارید چون یوزرآیدی که لاگین گرده با اطلاعات یوزر آیدی درخواستی یکی نیست");
            }

        }


        [HttpPut("{id}")]

        public async Task<IActionResult> UpdateUser(Guid id, UserForUpdateDto userForUpdateDto)
        {
            if (id.ToString() != User.FindFirst(ClaimTypes.NameIdentifier).Value)
            {
                return Unauthorized("شما اجازه ویرایش ندارید");
            }


            var userFromDb = await _db.UserRepository.GetByIdAsync(id);

            _mapper.Map(userForUpdateDto, userFromDb);
            _db.UserRepository.Update(userFromDb);

            if (await _db.SaveAsync())
            {
                return NoContent();
            }
            else
            {
                return BadRequest(new ReturnMessages()
                {
                    status = false,
                    title = "خطا",
                    message = $"ویرایش برای کاربر {userForUpdateDto.Name} انجام نشد"
                });
                //return Unauthorized("خطا در ویرایش ");
            }

        }


        public async Task<IActionResult> UserChangePassword(Guid id, PasswordForChangeDto passwordForChangeDto)
        {
            var userFromRepo = await _userService.GetUserForPassChange(id, passwordForChangeDto.OldPassword);
            if (userFromRepo == null)
            {
                return BadRequest(new ReturnMessages()
                {
                    status = false,
                    title = "خطا",
                    message = "پسورد قبلی اشتباه میباشد"
                });
            }

            if (await _userService.UpdateUserPass(userFromRepo, passwordForChangeDto.NewPassword))
            {
                return NoContent();
            }

            else
            {
                return BadRequest(new ReturnMessages()
                {
                    status = false,
                    title = "خطا",
                    message = "ویرایش پسورد کاربر انجام نشد"
                });
            }
        }





    }
}
