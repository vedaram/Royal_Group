

ALTER procedure [dbo].[spapproveall]

(@pialevel int,@pileave_id int,@pistat int,@pitxt varchar(max)=null,@piapprovedby varchar(100)=Null , @picomments varchar(max) = null)

as

begin

	if(@pialevel=0)

	begin

		if(@pitxt='leave')

		begin

			update leave1 set flag=@pistat,ApprovedbyName=@piapprovedby , Remarks  = @picomments where leave_id=@pileave_id

		end

		else

		begin

			update lossonpay set flag=@pistat,ApprovedbyName=@piapprovedby , Remarks  = @picomments where leave_id=@pileave_id

		end

	end

	else

	begin

		if(@pitxt='leave')

		begin

			update leave1 set flag=@pistat,ApprovedbyName=@piapprovedby , Remarks  = @picomments where leave_id=@pileave_id

		end

		else

		begin

			update lossonpay set flag=@pistat,ApprovedbyName=@piapprovedby , Remarks  = @picomments where leave_id=@pileave_id

		end

	end

end
